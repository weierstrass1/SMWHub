using FormatLibrary.Entries;
using FormatLibrary.Interfaces;
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
        private readonly ICommonListCategory _section;
        private int[] _ids => StateData.Get<int[]>("IDs")!;
        private FilePath[] _filepaths => StateData.Get<FilePath[]>("FileList")!;
        public CommonListParsingContext(LineContext context, ICommonListCategory section, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false) : base(context)
        {
            _section = section;
            StateData.AddStateVariable("Match", new MatchStateVariable("Match", _entryRegex));
            StateData.AddStateVariable("IDs", new IntegerIDListStateVariable<List<CommonListEntry>>(_entriesList, maxID, allowMultiIDs));
            StateData.AddStateVariable("FileList", new FilelistStateVariable(_section.BaseDirectory, allowVariables, allowMultiIDs));
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
                _entriesList.Add(id, []);
                _entriesList[id].Add(new()
                {
                    ID = id,
                    Category = _section,
                    Paths = _filepaths
                });
            }
        }
        public IEnumerable<CommonListEntry> GetEntries()
        {
            return _entriesList.SelectMany(cle => cle.Value);
        }
    }
}
