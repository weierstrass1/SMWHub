using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using System.Text.RegularExpressions;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class CommonListReader
{
    private sealed class CommonListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = RegexContainer.ListEntryRegex();
        private readonly Dictionary<int, List<CommonListEntry>> _entriesList = [];
        private readonly CommonListSectionTuple _section;
        private int[] _ids => State.Get<int[]>("IDs")!;
        private FilePath[] _filepaths => State.Get<FilePath[]>("FileList")!;
        public CommonListParsingContext(LineContext context, CommonListSectionTuple section, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false) : base(context)
        {
            _section = section;
            State.AddVariable("Match", new MatchStateVariable("Match", _entryRegex));
            State.AddVariable("IDs", new IntegerIDListStateVariable<List<CommonListEntry>>(_entriesList, maxID, allowMultiIDs));
            State.AddVariable("FileList", new FilelistStateVariable(_section.BaseDirectory, allowVariables, allowMultiIDs));
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            ValidationResult result = getSelfValidatedVariables(LineContext.LineContent);
            if (!result)
                return result;
            addEntries();
            return result;
        }
        private void addEntries()
        {
            foreach (int id in _ids)
            {
                _entriesList[id] = [];
                _entriesList[id].Add(new()
                {
                    ID = id,
                    EntryType = _section.Title,
                    Paths = _filepaths
                });
            }
        }
        public Dictionary<int, List<CommonListEntry>> GetEntries()
        {
            return _entriesList.ToDictionary();
        }
    }
}
