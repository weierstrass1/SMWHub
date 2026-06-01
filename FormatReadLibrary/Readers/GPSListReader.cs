using FormatReadLibrary.Entries;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using LogRegister;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed class GPSListReader(string baseDirectory)
{
    private readonly string _baseDirectory = baseDirectory;
    private readonly Dictionary<int, GPSListEntry> _entriesList = [];

    public bool Read(string path, LogRegisterSystem log)
    {
        FileReaderWithLog fReader = new(path, log);
        FileEnumeratorWithLog fileEnumerator = (FileEnumeratorWithLog)fReader.GetEnumerator()!;

        GPSListParserOptions opts = new()
        {
            EntriesList = _entriesList,
            FileEnumerator = fileEnumerator,
            BaseDirectory = _baseDirectory
        };

        GPSListParsingContext ctx = new(opts);

        while(fileEnumerator.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(fileEnumerator.Current))
                continue;
            if (!ctx.ProcessEntry())
                return false;
        }
        return true;
    }
    public IEnumerable<GPSListEntry> GetEntries()
    {
        return _entriesList.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
    }
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

            AddValidator(new ValidateEntryFormat(this, FileEnumerator));
            AddValidator(new ValidateFileExists(this,  FileEnumerator.Log));
            AddValidator(new ValidateEntryVariables(this, FileEnumerator));
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

            return addEntries(filepath, values, start, end, actlike);
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
        private bool addEntries(string filepath, int[]? values, int start, int end, int actlike)
        {
            int x = Math.Min(start % 16, end % 16);
            int y = start / 16;
            int xlimit = Math.Max(start % 16, end % 16);

            int currentIndex;
            for (int b = y; ; b++)
            {
                for (int a = x; a <= xlimit; a++)
                {
                    currentIndex = (b * 16) + a;
                    _entriesList[currentIndex] = new()
                    {
                        ID = currentIndex,
                        Values = values,
                        ActLike = actlike,
                        Path = filepath
                    };
                    if (currentIndex == end)
                        return true;
                }
            }
        }
    }
    private sealed class GPSListParserOptions
    {
        public required Dictionary<int, GPSListEntry> EntriesList { get; init; }
        public required FileEnumeratorWithLog FileEnumerator { get; init; }
        public required string BaseDirectory { get; init; }
    }
}
