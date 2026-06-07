using FormatReadLibrary.Entries;
using SMWHubEnumerators;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class GPSListReader(string baseDirectory)
{
    private readonly string _baseDirectory = baseDirectory;
    private readonly Dictionary<int, GPSListEntry> _entriesList = [];

    public ValidationResult Read(string path)
    {
        FileReader fReader = new(path);
        FileEnumerator fileEnumerator = (FileEnumerator)fReader.GetEnumerator()!;

        GPSListParserOptions opts = new()
        {
            EntriesList = _entriesList,
            Context = (LineContexts.FileEnumeratorLineContext)fileEnumerator,
            BaseDirectory = _baseDirectory
        };

        GPSListParsingContext ctx = new(opts);
        ValidationResult result = new();
        while (fileEnumerator.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(fileEnumerator.Current))
                continue;
            result.Merge(ctx.ProcessEntry());
        }
        return result;
    }
    public IEnumerable<GPSListEntry> GetEntries()
    {
        return _entriesList.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
    }
}
