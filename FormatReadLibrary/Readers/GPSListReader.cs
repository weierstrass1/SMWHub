using FormatReadLibrary.Entries;
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
        public GPSListParsingContext(GPSListParserOptions options) : base(options.FileEnumerator) 
        {
            State.AddVariable("BaseDirectory", new StateVariable<string>(options.BaseDirectory));
            State.AddVariable("Start", new StateVariable<int>());
            State.AddVariable("End", new StateVariable<int>());
            State.AddVariable("Entries", new StateVariable<Dictionary<int, GPSListEntry>>(options.EntriesList));
            State.AddVariable("Match", new LazyStateVariable<Match>(() =>
            {
                if (!FileEnumerator.IsValid())
                    return null;
                return _entryRegex.Match(FileEnumerator.Current);
            }));
            State.AddVariable("Filepath", new LazyStateVariable<string>(() =>
            {
                var match = State.Get<Match>("Match");
                if (match == null)
                    return null;
                var baseDirectory = State.Get<string>("BaseDirectory")!;
                return Path.Combine(baseDirectory, match!.Groups["file"].Value);
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
            AddValidator(new ValidateFileExists(this,  FileEnumerator.Log));
            AddValidator(new ValidateEntryVariables(this, FileEnumerator));
            _validateGPSBlockLine = new(this, FileEnumerator);
        }
        public override bool ProcessEntry()
        {
            if (!validate())
                return false;
            Match m = State.Get<Match>("Match")!;

            int start = Convert.ToInt32(m.Groups["idstart"].Value, 16);
            int end = start;
            if (m.Groups["idend"].Success)
                end = Convert.ToInt32(m.Groups["idend"].Value, 16);
            bool rectangle = m.Groups["r"].Success;
            if (end < start)
            {
                (start, end) = (end, start);
            }
            if (!rectangle)
            {
                State.Set("Start", start);
                State.Set("End", end);

                if (!_validateGPSBlockLine.Validate(this))
                    return false;
            }
            int x = Math.Min(start % 16, end % 16);
            int y = start / 16;
            int xlimit = Math.Max(start % 16, end % 16);
            int currentIndex;
            int actlike = -1;
            if (m.Groups["actlike"].Success)
                actlike = Convert.ToInt32(m.Groups["actlike"].Value, 16);
            
            for (int b = y; ; b++)
            {
                for (int a = x; a <= xlimit; a++)
                {
                    currentIndex = (b * 16) + a;
                    State.Get<Dictionary<int, GPSListEntry>>("Entries")![currentIndex] = new()
                    {
                        ID = currentIndex,
                        Values = State.Get<int[]>("Values")!,
                        ActLike = actlike,
                        Path = State.Get<string>("Filepath")!
                    };
                    if (currentIndex != end)
                        continue;
                    State.Reset();
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
