using FormatReadLibrary.Entries;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using LogRegister;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;
public sealed class CommonListReader
{
    private readonly Dictionary<string, CommonListSectionTuple> _sections;
    private readonly Dictionary<string, Dictionary<int, CommonListEntry>> _entriesList;
    public CommonListReader(CommonListSectionTuple[] sections)
    {
        _sections = [];
        _entriesList = [];
        string lowertitle;
        foreach (CommonListSectionTuple section in sections)
        {
            lowertitle = $"{section.Title.ToLower().Trim()}:";
            _sections.TryAdd(lowertitle, section);
            _entriesList.TryAdd(lowertitle, []);
        }
    }
    public bool Read(string path, LogRegisterSystem log, int maxID = 255, bool allowVariables = false)
    {
        FileReaderWithLog fReader = new(path, log);
        FileEnumeratorWithLog fileEnumerator = (FileEnumeratorWithLog)fReader.GetEnumerator()!;
        if(!fReader.SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, true, [.. _entriesList.Keys]))
            return false;

        CommonListParsingContext ctx;

        foreach (var section in enumerators)
        {
            ctx = new(section.Value, _sections[section.Key], maxID, allowVariables);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                if (!ctx.ProcessEntry())
                    return false;
            }
            _entriesList[section.Key] = ctx.GetEntries();
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
        private readonly CommonListSectionTuple _section;
        public CommonListParsingContext(FileEnumeratorWithLog fileEnumerator, CommonListSectionTuple section, int maxID = 255, bool allowVariables = false) : base(fileEnumerator)
        {
            _section = section;
            State.AddVariable("Match", new MatchStateVariable());
            State.AddVariable("ID", new StateVariable<int?>());
            State.AddVariable("Filepath", new StateVariable<string>());
            State.AddVariable("Values", new ValuesStateVariable());
            AddValidator(new ValidateEntryFormat(this, FileEnumerator));
            AddValidator(new ValidateEntryID(this, FileEnumerator, maxID));
            //AddValidator(new ValidateFileExists(this, FileEnumerator.Log));
            AddValidator(new ValidateEntryVariables(this, FileEnumerator, allowVariables));
            AddValidator(new ValidateDuplicateID<int, CommonListEntry>(this, FileEnumerator, _entriesList));
        }
        public override bool ProcessEntry()
        {
            setupStateVariables(out int id, out string filepath, out int[]? values);

            if (!validate())
                return false;

            _entriesList.Add(id, new()
            {
                ID = id,
                EntryType = _section.Title,
                Path = filepath,
                Values = values,
            });
            return true;
        }
        private void setupStateVariables(out int id, out string filepath, out int[]? values)
        {
            var matchVar = State.GetVariable<MatchStateVariable>("Match");
            Match match = matchVar.GetFrom(FileEnumerator.Current, _entryRegex)!;

            id = Convert.ToInt32(match.Groups["id"].Value, 16);
            State.Set("ID", id);

            filepath = Path.Combine(_section.BaseDirectory, match.Groups["file"].Value);
            State.Set("Filepath", filepath);

            var valuesVar = State.GetVariable<ValuesStateVariable>("Values");
            values = valuesVar.GetFrom(match);
        }
        public Dictionary<int, CommonListEntry> GetEntries()
        {
            return _entriesList.ToDictionary();
        }
    }
    public sealed record CommonListSectionTuple (string Title, string BaseDirectory);
}
