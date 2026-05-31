namespace FormatReadLibrary.Readers;

/*
public sealed class GPSListReader
{
    private readonly string _baseDirectory;
    private readonly Dictionary<int, GPSListEntry> _entriesList;
    public GPSListReader(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
        _entriesList = [];
    }
    public bool Read(string path, LogRegisterSystem log)
    {
        string content = FileUtils.CleanFileContent(path);

        string[] lines = content.Split('\n');

        GPSListParsingContext ctx = new(path, log, lines)
        {
            BaseDirectory = _baseDirectory,
            EntriesList = _entriesList,
        };

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;
            if (!ctx.ProcessEntry(i))
                return false;
        }
        return true;
    }
    public IEnumerable<GPSListEntry> GetEntries()
    {
        List<GPSListEntry> entries = [];
        foreach (var entry in _entriesList.OrderBy(kvp => kvp.Key))
        {
            entries.Add(entry.Value);
        }
        return entries;
    }
    private sealed class GPSListParsingContext
    {
        private static readonly Regex _entryRegex = FileRegexContainer.GPSListEntryRegex();

        public required string BaseDirectory { get; init; }
        public required Dictionary<int, GPSListEntry> EntriesList { get; init; }

        private readonly LogRegisterSystem _log;
        private readonly string _path;
        private readonly string[] _fileContentLines;

        private Match? _m;
        private string? _filepath;
        private int[]? _values;

        public GPSListParsingContext(string path, LogRegisterSystem log, string[] fileContentLines)
        {
            _path = path;
            _log = log;
            _fileContentLines = fileContentLines;
        }
        public bool ProcessEntry(int i)
        {
            if (!CommonValidations.ValidateEntryFormat(i, _path, _log, _fileContentLines, _entryRegex, out _m) ||
                !validateFileExists() ||
                !CommonValidations.ValidateEntryVariables(i, _path, _log, _fileContentLines, out _values, _m, true))
                return false;
            int start = Convert.ToInt32(_m.Groups["idstart"].Value, 16);
            int end = start;
            if (_m.Groups["idend"].Success)
                end = Convert.ToInt32(_m.Groups["idend"].Value, 16);
            bool rectangle = _m.Groups["r"].Success;
            if(end < start)
            {
                int tmp = end;
                end = start;
                start = tmp;
            }
            if (!rectangle)
            {
                if ($"{end:X2}"[..^1] != $"{start:X2}"[..^1])
                {
                    _log.Add(new SyntaxError(_path, i, _fileContentLines[i], $"Invalid Range ({start:X2}-{end:X2})"));
                    return false;
                }
            }
            int x = Math.Min(start % 16, end % 16);
            int y = start / 16;
            int xlimit = Math.Max(start % 16, end % 16);
            int currentIndex;
            int actlike = -1;
            if (_m.Groups["actlike"].Success)
                actlike = Convert.ToInt32(_m.Groups["actlike"].Value, 16);
            for (int b = y; ; b++)
            {
                for (int a = x; a <= xlimit; a++)
                {
                    currentIndex = (b * 16) + a;
                    EntriesList[currentIndex] = new()
                    {
                        ID = currentIndex,
                        Values = _values,
                        ActLike = actlike,
                        Path = _filepath!
                    };
                    if (currentIndex == end)
                        return true;
                }
            }
        }
        private bool validateFileExists()
        {
            _filepath = Path.Combine(BaseDirectory, _m!.Groups["file"].Value);
            return CommonValidations.ValidateFileExists(_path, _log, _filepath);
        }
    }
}
*/
