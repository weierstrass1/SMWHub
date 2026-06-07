using FormatReadLibrary.Entries;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed partial class GPSListReader
{
    private sealed class GPSListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = RegexContainer.GPSListEntryRegex();
        private readonly ValidateGPSBlockLine _validateGPSBlockLine;
        private readonly string _baseDirectory;
        private readonly Dictionary<int, GPSListEntry> _entriesList;
        private Match _match => State.Get<Match>("Match")!;
        private FilePath[] _filepaths => State.Get<FilePath[]>("Filelist")!;
        public GPSListParsingContext(GPSListParserOptions options) : base(options.FileEnumerator)
        {
            _baseDirectory = options.BaseDirectory;
            _entriesList = options.EntriesList;

            State.AddVariable("Start", new StateVariable<int>());
            State.AddVariable("End", new StateVariable<int>());
            State.AddVariable("Match", new MatchStateVariable("Match", _entryRegex));
            State.AddVariable("Filelist", new FilelistStateVariable(_baseDirectory, true, true));

            _validateGPSBlockLine = new(this);
        }
        public override bool ProcessEntry()
        {
            if (!getSelfValidatedVariables(FileEnumerator.Current))
                return false;

            setupEntryRange(_match, out int start, out int end, out bool rectangle);

            if (!rectangle && !validateStartEnd(start, end))
                return false;

            int actlike = _match.Groups["actlike"].Success ?
                actlike = Convert.ToInt32(_match.Groups["actlike"].Value, 16) :
                -1;

            foreach(var filepath in _filepaths)
            {
                addEntries(filepath.Path, filepath.Values, start, end, actlike);
            }
            return true;
        }
        private static void setupEntryRange(Match match, out int start, out int end, out bool rectangle)
        {
            start = Convert.ToInt32(match.Groups["idstart"].Value, 16);
            end = start;
            if (match.Groups["idend"].Success)
                end = Convert.ToInt32(match.Groups["idend"].Value, 16);
            rectangle = match.Groups["r"].Success;
            if (end <= start)
                return;
            (start, end) = (end, start);
        }
        private bool validateStartEnd(int start, int end)
        {
            State.Set("Start", start);
            State.Set("End", end);

            ValidationResult result = _validateGPSBlockLine.Validate(this);
            if (!result)
                ValidatorLogAdapter.LogValidatorResult(FileEnumerator, result);

            return result;
        }
        private void addEntries(string filepath, int[]? values, int start, int end, int actlike)
        {
            int startCol = start % 16;
            int endCol = end % 16;
            int startRow = start / 16;
            int endRow = end / 16;

            int leftCol = Math.Min(startCol, endCol);
            int rightCol = Math.Max(startCol, endCol);

            for (int row = startRow; row <= endRow; row++)
            {
                for (int col = leftCol; col <= rightCol; col++)
                {
                    int index = (row * 16) + col;

                    _entriesList[index] = new GPSListEntry
                    {
                        ID = index,
                        Values = values,
                        ActLike = actlike,
                        Path = filepath
                    };
                }
            }
        }
    }
}
