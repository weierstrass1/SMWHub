using FormatReadLibrary.Infos;
using FormatReadLibrary.Logging.LoggingRegisters;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using LogRegister;
using StateMachine;
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

        dynamicInfo = null;
        if (!fReader.SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, true,
            "posesgraphics:", "palettes:", "resources:", "poseschunkssizes:", "numberof16x16tilesperpose:"))
            return false;

        if (!validateIfUseBothFormats(name, log, fReader, enumerators))
            return false;

        ParsingContext ctx;
        dynamicInfo = new(Path.GetFileNameWithoutExtension(name));

        foreach (var section in enumerators)
        {
            ctx = createContext(section.Key, dynamicInfo, section.Value);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                if (!ctx.ProcessEntry())
                    return false;
            }
        }

        return true;
    }
    private static bool validateIfUseBothFormats(string name, LogRegisterSystem log, FileReaderWithLog fReader, Dictionary<string, FileEnumeratorWithLog> enumerators)
    {
        if (enumerators.TryGetValue("poseschunkssizes:", out FileEnumeratorWithLog? legacyFormat) &&
            enumerators.TryGetValue("numberof16x16tilesperpose:", out FileEnumeratorWithLog? currentFormat))
        {
            int i = Math.Max(legacyFormat.LineIndex, currentFormat.LineIndex);
            log.Add(new SyntaxError(i, name, fReader[i], $"Both 'poseschunkssizes:' and 'numberof16x16tilesperpose:' sections are present. You can't use legacy and current format at the same time."));
            return false;
        }
        return true;
    }
    private ParsingContext createContext(string section, DynamicInfo dynamicInfo, FileEnumeratorWithLog fileEnumerator)
    {
        return section switch
        {
            "posesgraphics:" => new DynamicInfoResourceListParsingContext(fileEnumerator, DynamicInfoSection.PosesGraphics)
            { DynamicInfo = dynamicInfo },
            "palettes:" => new DynamicInfoResourceListParsingContext(fileEnumerator, DynamicInfoSection.Palettes)
            { DynamicInfo = dynamicInfo },
            "resources:" => new DynamicInfoResourceListParsingContext(fileEnumerator, DynamicInfoSection.Resources)
            { DynamicInfo = dynamicInfo },
            "poseschunkssizes:" => new DynamicInfoLegacyFormatParsingContext(fileEnumerator)
            { DynamicInfo = dynamicInfo },
            "numberof16x16tilesperpose:" => new DynamicInfoCurrentFormatParsingContext(fileEnumerator)
            { DynamicInfo = dynamicInfo },
            _ => throw new Exception($"Unknown section type: {section}")
        };
    }
    private enum DynamicInfoSection
    {
        PosesGraphics,
        Palettes,
        Resources,
        PosesChunkSizes,
        NumberOf16x16TilesPerPose
    }
    private sealed class DynamicInfoResourceListParsingContext : ParsingContext, IHaveDynamicInfo
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private readonly List<string> _list = [];
        private readonly DynamicInfoSection _section;
        public DynamicInfoResourceListParsingContext(FileEnumeratorWithLog fileEnumerator, DynamicInfoSection section) : base(fileEnumerator)
        {
            _section = section;
            AddValidator(new ValidatePathIntegrity(this, FileEnumerator));
        }
        public override bool ProcessEntry()
        {
            if (!validate())
                return false;

            _list.Add(FileEnumerator.Current);
            if (!FileEnumerator.IsLastLine())
                return true;

            setupDynamicInfoList();
            return true;
        }
        private void setupDynamicInfoList()
        {
            string[] arr = [.. _list];

            switch (_section)
            {
                case DynamicInfoSection.PosesGraphics:
                    DynamicInfo.PoseGraphics = arr;
                    break;
                case DynamicInfoSection.Palettes:
                    DynamicInfo.Palettes = arr;
                    break;
                case DynamicInfoSection.Resources:
                    DynamicInfo.Resources = arr;
                    break;
            }
        }
    }
    private sealed class DynamicInfoLegacyFormatParsingContext : ParsingContext, IHaveDynamicInfo
    {
        public required DynamicInfo DynamicInfo { get; init; }
        public bool Active { get; private set; } = false;
        private static Regex _entryRegex = FileRegexContainer.DynInfoLegacyRegex();
        private static Regex _entryTableRegex = FileRegexContainer.NumberTableRegex();
        private readonly Dictionary<string, (int, int)> _poseChunkSizes = [];
        private readonly ValidateEntryFormat _validateEntryFormat;
        private readonly ValidateIfHasNext _ifHasNext;
        private readonly ValidateDuplicateID<string, (int, int)> _validateDuplicateID;
        public DynamicInfoLegacyFormatParsingContext(FileEnumeratorWithLog fileEnumerator) : base(fileEnumerator)
        {
            State.AddVariable("Match", new MatchStateVariable());
            State.AddVariable("MatchTable", new MatchStateVariable());
            State.AddVariable("ID", new StateVariable<string>());
            State.AddVariable("Values", new ValuesStateVariable());
            _validateEntryFormat = new ValidateEntryFormat(this, FileEnumerator);
            _validateDuplicateID = new ValidateDuplicateID<string, (int, int)>(this, FileEnumerator, _poseChunkSizes);
            _ifHasNext = new ValidateIfHasNext(this, FileEnumerator);
            AddValidator(new ValidateTableValueSize(this, FileEnumerator, TableValueSize.db));
            AddValidator(new ValidateEntryFormat(this, FileEnumerator, "MatchTable"));
            AddValidator(new ValidateValuesSize(this, FileEnumerator, 2, 2));

        }
        public override bool ProcessEntry()
        {
            string id;
            if (!validatePoseChunksTitle(out id))
                return false;

            if (!_ifHasNext.Validate(this))
                return false;

            int[]? values = setupValues();
            if (!validate())
                return false;

            Active = true;
            _poseChunkSizes.Add(id, (values![0], values[1]));
            if (!FileEnumerator.IsLastLine())
                return true;

            setupDynamicInfoPosesChunksSizes();
            return true;
        }
        private int[]? setupValues()
        {
            var matchTableVar = State.GetVariable<MatchStateVariable>("MatchTable");
            matchTableVar.GetFrom(FileEnumerator.Current, _entryTableRegex);
            var valuesVar = State.GetVariable<ValuesStateVariable>("Values");
            return valuesVar.GetFrom(FileEnumerator.Current);
        }
        private bool validatePoseChunksTitle(out string id)
        {
            var matchVar = State.GetVariable<MatchStateVariable>("Match");
            Match match = matchVar.GetFrom(FileEnumerator.Current, _entryRegex)!;

            id = "";

            if (!_validateEntryFormat.Validate(this))
                return false;

            id = match.Groups["id"].Value;
            State.Set("ID", id);

            return _validateDuplicateID.Validate(this);
        }

        private void setupDynamicInfoPosesChunksSizes()
        {
            List<int> pcs = [];
            foreach (var tuple in _poseChunkSizes.Values)
                pcs.AddRange([tuple.Item1, tuple.Item2]);
            DynamicInfo.PosesChunksSizes = [.. pcs];
            DynamicInfo.GenerateLastRow();
        }
    }
    private sealed class DynamicInfoCurrentFormatParsingContext : ParsingContext, IHaveDynamicInfo
    {
        public required DynamicInfo DynamicInfo { get; init; }
        public bool Active { get; private set; } = false;
        private readonly Dictionary<int, string> _currentNumberOf16x16TilesPerPose = [];

        public DynamicInfoCurrentFormatParsingContext(FileEnumeratorWithLog fileEnumerator) : base(fileEnumerator)
        {
        }

        public override bool ProcessEntry()
        {
            throw new NotImplementedException();
        }
    }
}
