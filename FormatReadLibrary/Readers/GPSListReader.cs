using FormatReadLibrary.Entries;
using FormatReadLibrary.Logging.Enumerators;
using LogRegister;

namespace FormatReadLibrary.Readers;

public sealed partial class GPSListReader(string baseDirectory)
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
    private sealed class GPSListParserOptions
    {
        public required Dictionary<int, GPSListEntry> EntriesList { get; init; }
        public required FileEnumeratorWithLog FileEnumerator { get; init; }
        public required string BaseDirectory { get; init; }
    }
}
