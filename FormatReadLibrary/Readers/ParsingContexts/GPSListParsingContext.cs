using FormatReadLibrary.Entries;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed partial class GPSListReader
{
    private sealed class GPSListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = FileRegexContainer.GPSListEntryRegex();
        private readonly ValidateGPSBlockLine _validateGPSBlockLine;
        private readonly string _baseDirectory;
        private readonly Dictionary<int, GPSListEntry> _entriesList;
        public GPSListParsingContext(GPSListParserOptions options) : base(options.FileEnumerator)
        {
            _baseDirectory = options.BaseDirectory;
            _entriesList = options.EntriesList;

            State.AddVariable("Start", new StateVariable<int>());
            State.AddVariable("End", new StateVariable<int>());
            State.AddVariable("Match", new MatchStateVariable());
            State.AddVariable("Filepath", new StateVariable<string>());
            State.AddVariable("Values", new ValuesStateVariable());

            addValidator(new ValidateEntryFormat(this, FileEnumerator));
            //AddValidator(new ValidateFileExists(this,  FileEnumerator.Log));
            addValidator(new ValidateEntryVariables(this, FileEnumerator));
            _validateGPSBlockLine = new(this, FileEnumerator);
        }
        public override bool ProcessEntry()
        {
            setupStateVariables(out Match match, out string filepath, out int[]? values);

            if (!validate())
                return false;

            setupEntryRange(match, out int start, out int end, out bool rectangle);

            if (!rectangle && !validateStartEnd(start, end))
                return false;

            int actlike = match.Groups["actlike"].Success ?
                actlike = Convert.ToInt32(match.Groups["actlike"].Value, 16) :
                -1;

            addEntries(filepath, values, start, end, actlike);
            return true;
        }
        private void setupStateVariables(out Match match, out string filepath, out int[]? values)
        {
            var matchVar = State.GetVariable<MatchStateVariable>("Match");
            match = matchVar.GetFrom(FileEnumerator.Current, _entryRegex)!;
            filepath = Path.Combine(_baseDirectory, match.Groups["file"].Value);
            State.Set("Filepath", filepath);
            var valuesVar = State.GetVariable<ValuesStateVariable>("Values");
            values = valuesVar.GetFrom(match);
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
}
