using FormatReadLibrary.Entries;
using FormatReadLibrary.Logging.Enumerators;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using System.Text.RegularExpressions;

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
        public CommonListParsingContext(FileEnumeratorWithLog fileEnumerator, CommonListSectionTuple section, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false) : base(fileEnumerator)
        {
            _section = section;
            State.AddVariable("Match", new MatchStateVariable("Match", _entryRegex));
            State.AddVariable("IDs", new IntegerIDListStateVariable<List<CommonListEntry>>(_entriesList));
            State.AddVariable("FileList", new FilelistStateVariable(_section.BaseDirectory, allowVariables, allowMultiIDs));
        }
        public override bool ProcessEntry()
        {
            if (!getSelfValidatedVariables(FileEnumerator.Current))
                return false;
            addEntries();
            return true;
        }
        private void addEntries()
        {
            foreach (int id in _ids)
            {
                _entriesList[id] = [];
                foreach (var filepath in _filepaths)
                {
                    _entriesList[id].Add(new()
                    {
                        ID = id,
                        EntryType = _section.Title,
                        Path = filepath.Path,
                        Values = filepath.Values,
                    });
                }
            }
        }
        public Dictionary<int, List<CommonListEntry>> GetEntries()
        {
            return _entriesList.ToDictionary();
        }
    }
}
