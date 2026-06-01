using FormatReadLibrary.Entries;
using FormatReadLibrary.Readers.Validators;
using LogRegister;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;
public sealed class CommonListReader
{
    private readonly Dictionary<string, string> _baseDirectories;
    private readonly Dictionary<string, Dictionary<int, CommonListEntry>> _entriesList;
    public CommonListReader((string, string)[] titles)
    {
        _baseDirectories = [];
        _entriesList = [];
        string lowertitle;
        foreach ((string title, string baseDir) in titles)
        {
            lowertitle = $"{title.ToLower().Trim()}:";
            _baseDirectories.TryAdd(lowertitle, baseDir);
            _entriesList.TryAdd(lowertitle, []);
        }
    }
    public bool Read(string path, LogRegisterSystem log, int maxID = 255, bool allowVariables = false)
    {
        FileReaderWithLog fReader = new(path, log);
        FileEnumeratorWithLog fileEnumerator = (FileEnumeratorWithLog)fReader.GetEnumerator()!;

        CommonListParserOptions opts = new()
        {
            Sections = _entriesList.Keys,
            FileEnumerator = fileEnumerator,
            BaseDirectories = _baseDirectories,
            EntriesList = _entriesList,
        };

        CommonListParsingContext ctx = new(opts, maxID, allowVariables);

        while(fileEnumerator.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(fileEnumerator.Current))
                continue;
            if (!ctx.ProcessEntry(fileEnumerator))
                return false;
        }
        return true;
    }
    public IEnumerable<CommonListEntry> GetEntries()
    {
        List<CommonListEntry> entries = [];
        foreach (var entry in _entriesList)
        {
            entries.AddRange(entry.Value.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));
        }
        return entries;
    }
    private sealed class CommonListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = FileRegexContainer.ListEntryRegex();
        private readonly Dictionary<string, Dictionary<int, CommonListEntry>> _entriesList;
        private readonly Dictionary<string, string> _baseDirectories;
        private readonly ValidateSectionIsNotRepeated _sectionIsNotRepeated;
        private string? _currentSection;
        private readonly Dictionary<string, bool> _processedSections;
        public CommonListParsingContext(CommonListParserOptions options, int maxID = 255, bool allowVariables = false) : base()
        {
            _processedSections = options.Sections.ToDictionary(k => k, k => false);
            _baseDirectories = options.BaseDirectories;
            _entriesList = options.EntriesList;
            State.AddVariable("BaseDirectory", new StateVariable<string>());
            State.AddVariable("Entries", new StateVariable<Dictionary<int, CommonListEntry>>());
            State.AddVariable("SectionWasProcessed", new StateVariable<bool>());
            State.AddVariable("Match", new LazyStateVariable<Match>(() =>
            {
                if (options.FileEnumerator.LineIndex < 0)
                    return null;
                return _entryRegex.Match(options.FileEnumerator.Current);
            }));
            State.AddVariable("ID", new LazyStateVariable<int?>(() =>
            {
                var match = State.Get<Match>("Match");
                if (match == null)
                    return null;
                return Convert.ToInt32(match.Groups["id"].Value, 16);
            }));
            State.AddVariable("Filepath", new LazyStateVariable<string>(() =>
            {
                var match = State.Get<Match>("Match");
                if (match == null)
                    return null;
                var baseDirectory = State.Get<string>("BaseDirectory")!;
                return Path.Combine(baseDirectory, match!.Groups["file"].Value);
            }));
            State.AddVariable("Values", new LazyStateVariable<int[]>(() =>
            {
                var match = State.Get<Match>("Match");
                if (match == null)
                    return null;
                if (!match.Groups["var"].Success)
                    return [];
                return [..match.Groups["var"].Value
                    .Split(' ')
                    .Select(x => x[0] == '@' ?
                        int.Parse(x[1..]) :
                        Convert.ToInt32(x, 16))];
            }));
            _sectionIsNotRepeated = new(this, options.FileEnumerator);
            AddValidator(new ValidateListContext(this, options.FileEnumerator));
            AddValidator(new ValidateEntryFormat(this, options.FileEnumerator));
            AddValidator(new ValidateEntryID(this, options.FileEnumerator, maxID));
            AddValidator(new ValidateFileExists(this, options.FileEnumerator.Log));
            AddValidator(new ValidateEntryVariables(this, options.FileEnumerator, allowVariables));
            AddValidator(new ValidateDuplicateID<int, CommonListEntry>(this, options.FileEnumerator));
        }
        public override bool ProcessEntry(FileEnumeratorWithLog fileEnumerator)
        {
            string lowerLine = fileEnumerator.Current.ToLower().Trim();
            if (isATitle(lowerLine))
            {
                if (!_sectionIsNotRepeated.Validate(this))
                    return false;
                State.Set("BaseDirectory", _baseDirectories[lowerLine]);
                State.Set("Entries", _entriesList[lowerLine]);
                State.Set("SectionWasProcessed", _processedSections[lowerLine]);
                _currentSection = lowerLine;
                return true;
            }
            if (!validate())
                return false;
            var entries = State.Get<Dictionary<int, CommonListEntry>>("Entries")!;
            var id = State.Get<int>("ID");
            entries.Add(id, new()
            {
                EntryType = _currentSection!.Replace(":", ""),
                ID = id,
                Path = State.Get<string>("Filepath")!,
                Values = State.Get<int[]>("Values")!,
            });
            State.Reset();
            return true;
        }
        private bool isATitle(string lowerLine)
        {
            return _entriesList.ContainsKey(lowerLine);
        }
    }
    private sealed class CommonListParserOptions
    {
        public required IEnumerable<string> Sections { get; init; }
        public required FileEnumeratorWithLog FileEnumerator { get; init; }
        public required Dictionary<string, Dictionary<int, CommonListEntry>> EntriesList { get; init; }
        public required Dictionary<string, string> BaseDirectories { get; init; }
    }
}
