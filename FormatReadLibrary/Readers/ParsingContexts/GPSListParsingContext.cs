using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using SMWHubValidations.StateVariableValidations;
using System.Text.RegularExpressions;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class GPSListReader
{
    private sealed class GPSListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = RegexContainer.GPSListEntryRegex();
        private readonly ValidateGPSBlockLine _validateGPSBlockLine;
        private readonly string _baseDirectory;
        private readonly Dictionary<int, GPSListEntry> _entriesList;
        private Match _match => StateData.Get<Match>("Match")!;
        private FilePath[] _filepaths => StateData.Get<FilePath[]>("Filelist")!;
        public GPSListParsingContext(GPSListParserOptions options) : base(options.Context)
        {
            _baseDirectory = options.BaseDirectory;
            _entriesList = options.EntriesList;

            StateData.AddVariable<int>("Start");
            StateData.AddVariable<int>("End");
            StateData.AddStateVariable("Match", new MatchStateVariable("Match", _entryRegex));
            StateData.AddStateVariable("Filelist", new FilelistStateVariable(_baseDirectory, true, true));

            _validateGPSBlockLine = new(this);
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            ValidationResult result = getSelfValidatedVariables(LineContext.LineContent);
            if (!result)
                return result;

            setupEntryRange(_match, out int start, out int end, out bool rectangle);

            if (!rectangle)
            {
                result = validateStartEnd(start, end);
                if (!result)
                    return result;
            }
            int actlike = _match.Groups["actlike"].Success ?
                Convert.ToInt32(_match.Groups["actlike"].Value, 16) :
                -1;

            foreach (var filepath in _filepaths)
            {
                addEntries(filepath.Path, filepath.Values, start, end, actlike);
            }
            return result;
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
        private ValidationResult validateStartEnd(int start, int end)
        {
            StateData.Set("Start", start);
            StateData.Set("End", end);

            return _validateGPSBlockLine.Validate(this);
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
    private sealed class GPSListParserOptions
    {
        public required Dictionary<int, GPSListEntry> EntriesList { get; init; }
        public required FileEnumeratorLineContext Context { get; init; }
        public required string BaseDirectory { get; init; }
    }
}
