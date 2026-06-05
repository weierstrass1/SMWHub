using FormatReadLibrary.Entries;
using FormatReadLibrary.Logging.Enumerators;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed partial class CommonListReader
{
    private sealed class CommonListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = FileRegexContainer.ListEntryRegex();
        private readonly Dictionary<int, List<CommonListEntry>> _entriesList = [];
        private readonly CommonListSectionTuple _section;
        public CommonListParsingContext(FileEnumeratorWithLog fileEnumerator, CommonListSectionTuple section, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false) : base(fileEnumerator)
        {
            _section = section;
            State.AddVariable("Match", new MatchStateVariable());
            State.AddVariable("IDs", new ValuesStateVariable());
            State.AddVariable("FileList", new FilelistStateVariable(fileEnumerator));
            State.AddVariable("Filepath", new FilepathStateVariable());
            State.AddVariable("Values", new ValuesStateVariable());

            addValidator(new ValidateEntryFormat(this, fileEnumerator));
            addValidator(new ValidateEntryID(this, fileEnumerator, maxID));
            //AddValidator(new ValidateFileExists(this, FileEnumerator.Log));
            addValidator(new ValidateEntryVariables(this, fileEnumerator, allowVariables));
            addValidator(new ValidateDuplicateID<int, List<CommonListEntry>>(this, fileEnumerator, _entriesList, allowMultiIDs));
        }
        public override bool ProcessEntry()
        {
            setupStateVariables(out int[] ids, out string filepath, out int[]? values);

            if (!validate())
                return false;

            foreach (int id in ids)
            {
                _entriesList[id].Add(new()
                {
                    ID = id,
                    EntryType = _section.Title,
                    Path = filepath,
                    Values = values,
                });
            }
            return true;
        }
        private void setupStateVariables(out int[] ids, out string filepath, out int[]? values)
        {
            var matchVar = State.GetVariable<MatchStateVariable>("Match");
            Match match = matchVar.GetFrom(FileEnumerator.Current, _entryRegex)!;

            var idsVar = State.GetVariable<ValuesStateVariable>("Values");
            ids = idsVar.GetFrom(match)!;

            filepath = Path.Combine(_section.BaseDirectory, match.Groups["file"].Value);
            State.Set("Filepath", filepath);

            var valuesVar = State.GetVariable<ValuesStateVariable>("Values");
            values = valuesVar.GetFrom(match);
        }
        public Dictionary<int, List<CommonListEntry>> GetEntries()
        {
            return _entriesList.ToDictionary();
        }
    }
}
