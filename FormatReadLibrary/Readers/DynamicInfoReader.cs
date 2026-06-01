using Configs;
using FormatReadLibrary.Infos;
using FormatReadLibrary.Logging.LoggingRegisters;
using FormatReadLibrary.Readers.Validators;
using LogRegister;
using StateMachine;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed class DynamicInfoReader
{
    public bool Read(string path, LogRegisterSystem log, out DynamicInfo? dynamicInfo)
    {
        string content = File.ReadAllText(path);
        return Read(Path.GetFileNameWithoutExtension(path), content, log, out dynamicInfo);
    }
    public bool Read(string name, string dynamicInfoContent, LogRegisterSystem log, out DynamicInfo? dynamicInfo)
    {
        FileReaderWithLog fReader = new(name, dynamicInfoContent, log);
        FileEnumeratorWithLog fileEnumerator = (FileEnumeratorWithLog)fReader.GetEnumerator()!;

        DynamicInfoParsingContext ctx = new(fileEnumerator);

        while (fileEnumerator.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(fileEnumerator.Current))
                continue;
            if (!ctx.ProcessEntry(fileEnumerator))
            {
                dynamicInfo = null;
                return false;
            }
        }
        dynamicInfo = ctx.GetDynamicInfo(name);
        return true;
    }
    private sealed class DynamicInfoParsingContext : ParsingContext
    {
        private readonly List<string> _poseGraphics = [];
        private readonly List<string> _palettes = [];
        private readonly List<string> _resources = [];
        private readonly Dictionary<string, (int, int)> _poseChunkSizes = [];
        private readonly Dictionary<int, string> _currentNumberOf16x16TilesPerPose = [];
        private readonly DynamicInfoResourceListParsingContext _poseGraphicsListPC;
        private readonly DynamicInfoResourceListParsingContext _palettesListPC;
        private readonly DynamicInfoResourceListParsingContext _resourcesListPC;
        private readonly DynamicInfoLegacyFormatParsingContext _legacyFormatPC;
        private readonly DynamicInfoCurrentFormatParsingContext _currentFormatPC;
        private string? _currentSection;
        private readonly Dictionary<string, bool> _processedSections = new(){
                { "posesgraphics:", false },
                { "palettes:", false },
                { "resources:", false },
                { "poseschunkssizes:", false },
                { "numberof16x16tilesperpose:", false }
            };
        private readonly Dictionary<string, Func<FileEnumeratorWithLog, bool>> _sectionProcessing;
        private readonly ValidateListContext _validateListContext;
        private readonly ValidateSectionIsNotRepeated _sectionIsNotRepeated;
        public DynamicInfoParsingContext(FileEnumeratorWithLog fileEnumerator)
        {
            _poseGraphicsListPC = new(fileEnumerator, _poseGraphics);
            _palettesListPC = new(fileEnumerator, _palettes);
            _resourcesListPC = new(fileEnumerator, _resources);
            _legacyFormatPC = new(fileEnumerator, _poseChunkSizes);
            _currentFormatPC = new(fileEnumerator, _currentNumberOf16x16TilesPerPose);
            _sectionProcessing = new(){
                { "posesgraphics:", _poseGraphicsListPC.ProcessEntry },
                { "palettes:", _palettesListPC.ProcessEntry },
                { "resources:", _resourcesListPC.ProcessEntry },
                { "poseschunkssizes:", _legacyFormatPC.ProcessEntry },
                { "numberof16x16tilesperpose:", _currentFormatPC.ProcessEntry }
            };
            State.AddVariable("SectionWasProcessed", new StateVariable<bool>());
            _sectionIsNotRepeated = new(this, fileEnumerator);
            _validateListContext = new(this, fileEnumerator);
        }
        public override bool ProcessEntry(FileEnumeratorWithLog fileEnumerator)
        {
            string lowerLine = fileEnumerator.Current.ToLower().Trim();
            if (isSectionTitle(lowerLine))
            {
                if (!_sectionIsNotRepeated.Validate(this))
                    return false;
                State.Set("SectionWasProcessed", _processedSections[lowerLine]);
                _currentSection = lowerLine;
                return true;
            }
            if (!_validateListContext.Validate(this))
                return false;
            return _sectionProcessing[_currentSection!].Invoke(fileEnumerator);
        }
        public DynamicInfo GetDynamicInfo(string name)
        {
            DynamicInfo di = new(name)
            {
                Palettes = [.. _palettes],
                PoseGraphics = [.. _poseGraphics],
                Resources = [.. _resources]
            };
            if (_legacyFormatPC.Active)
            {
                List<int> pcs = [];
                foreach (var tuple in _poseChunkSizes.Values)
                    pcs.AddRange([tuple.Item1, tuple.Item2]);
                di.PosesChunksSizes = [.. pcs];
                di.GenerateLastRow();
            }
            else if (_currentFormatPC.Active)
            {
                di.FromNumberOf16x16Tiles(_currentNumberOf16x16TilesPerPose);
            }
            return di;
        }
        private bool isSectionTitle(string sectionTitle)
        {
            return _processedSections.ContainsKey(sectionTitle);
        }
    }
    private sealed class DynamicInfoResourceListParsingContext : ParsingContext
    {
        private readonly List<string> _list = [];
        public DynamicInfoResourceListParsingContext(FileEnumeratorWithLog fileEnumerator, List<string> list)
        {
            _list = list;
            AddValidator(new ValidatePathIntegrity(this, fileEnumerator));
        }
        public override bool ProcessEntry(FileEnumeratorWithLog fileEnumerator)
        {
            if (!validate())
                return false;
            _list.Add(fileEnumerator.Current);
            return true;
        }
    }
    private sealed class DynamicInfoLegacyFormatParsingContext : ParsingContext
    {
        public bool Active { get; private set; } = false;
        private static Regex _entryRegex = FileRegexContainer.DynInfoLegacyRegex();
        private static Regex _entryTableRegex = FileRegexContainer.NumberTableRegex();
        private readonly Dictionary<string, (int, int)> _poseChunkSizes;
        private readonly ValidateEntryFormat _validateEntryFormat;
        private readonly ValidateIfHasNext _ifHasNext;
        private readonly ValidateDuplicateID<string, (int, int)> _validateDuplicateID;
        public DynamicInfoLegacyFormatParsingContext(FileEnumeratorWithLog fileEnumerator, Dictionary<string, (int, int)> poseChunkSizes)
        {
            _poseChunkSizes = poseChunkSizes;
            State.AddVariable("Entries", new StateVariable<Dictionary<string, (int, int)>>(_poseChunkSizes));
            State.AddVariable("Match", new LazyStateVariable<Match>(() =>
            {
                if (fileEnumerator.LineIndex < 0)
                    return null;
                return _entryRegex.Match(fileEnumerator.Current);
            }));
            State.AddVariable("MatchTable", new LazyStateVariable<Match>(() =>
            {
                if (fileEnumerator.LineIndex < 0)
                    return null;
                return _entryTableRegex.Match(fileEnumerator.Current);
            }));
            State.AddVariable("ID", new LazyStateVariable<string>(() =>
            {
                var match = State.Get<Match>("Match");
                if (match == null)
                    return null;
                return match.Groups["id"].Value;
            }));
            State.AddVariable("Values", new LazyStateVariable<int[]>(() =>
            {
                return HexUtils.GetValues(fileEnumerator.Current);
            }));
            _validateEntryFormat = new ValidateEntryFormat(this, fileEnumerator);
            _validateDuplicateID = new ValidateDuplicateID<string, (int, int)>(this, fileEnumerator);
            _ifHasNext = new ValidateIfHasNext(this, fileEnumerator);
            AddValidator(new ValidateTableValueSize(this, fileEnumerator, TableValueSize.db));
            AddValidator(new ValidateEntryFormat(this, fileEnumerator, "MatchTable"));
            AddValidator(new ValidateValuesSize(this, fileEnumerator, 2, 2));

        }
        public override bool ProcessEntry(FileEnumeratorWithLog fileEnumerator)
        {
            if (!_validateEntryFormat.Validate(this))
                return false;
            if (!_validateDuplicateID.Validate(this))
                return false;
            Match m = State.Get<Match>("Match")!;
            string id = m.Groups["id"].Value;
            if (!_ifHasNext.Validate(this))
                return false;                
            if (!validate())
                return false;
            Active = true;
            var values = State.Get<int[]>("Values")!;
            _poseChunkSizes.Add(id, (values[0], values[1]));
            State.Reset();
            return true;
        }
    }
    private sealed class DynamicInfoCurrentFormatParsingContext : ParsingContext
    {
        public bool Active { get; private set; } = false;
        private readonly Dictionary<int, string> _currentNumberOf16x16TilesPerPose;
        public DynamicInfoCurrentFormatParsingContext(FileEnumeratorWithLog fileEnumerator, Dictionary<int, string> currentNumberOf16x16TilesPerPose)
        {
            _currentNumberOf16x16TilesPerPose = currentNumberOf16x16TilesPerPose;
        }
        public override bool ProcessEntry(FileEnumeratorWithLog fileEnumerator)
        {
            throw new NotImplementedException();
        }
    }
}
