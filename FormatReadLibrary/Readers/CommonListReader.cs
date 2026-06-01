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
        fReader.SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, [.. _entriesList.Keys]);

        CommonListParsingContext ctx;
        Dictionary<int, CommonListEntry> newEntries;

        foreach (var section in enumerators)
        {
            ctx = new(section.Value, maxID, allowVariables);
            section.Value.MoveNext();
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                if (!ctx.ProcessEntry())
                    return false;
            }
            newEntries = ctx.GetEntries();
            foreach (var entry in newEntries.Values)
            {
                entry.EntryType = section.Key;
                entry.Path = Path.Combine(_baseDirectories[section.Key], entry.Path);
            }
            _entriesList[section.Key] = newEntries;
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
        private readonly Dictionary<int, CommonListEntry> _entriesList = [];
        public CommonListParsingContext(FileEnumeratorWithLog fileEnumerator, int maxID = 255, bool allowVariables = false) : base(fileEnumerator)
        {
            State.AddVariable("Match", new LazyStateVariable<Match>(() =>
            {
                if (!FileEnumerator.IsValid())
                    return null;
                return _entryRegex.Match(FileEnumerator.Current);
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
                return match!.Groups["file"].Value;
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
            AddValidator(new ValidateEntryFormat(this, FileEnumerator));
            AddValidator(new ValidateEntryID(this, FileEnumerator, maxID));
            //AddValidator(new ValidateFileExists(this, FileEnumerator.Log));
            AddValidator(new ValidateEntryVariables(this, FileEnumerator, allowVariables));
            AddValidator(new ValidateDuplicateID<int, CommonListEntry>(this, FileEnumerator, _entriesList));
        }
        public override bool ProcessEntry()
        {
            if (!validate())
                return false;
            var id = State.Get<int>("ID");
            _entriesList.Add(id, new()
            {
                ID = id,
                Path = State.Get<string>("Filepath")!,
                Values = State.Get<int[]>("Values")!,
            });
            State.Reset();
            return true;
        }
        public Dictionary<int, CommonListEntry> GetEntries()
        {
            return _entriesList.ToDictionary();
        }
    }
}
