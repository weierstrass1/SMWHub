using FormatReadLibrary.Entries;
using FormatReadLibrary.Logging.Enumerators;
using LogRegister;

namespace FormatReadLibrary.Readers;
public sealed partial class CommonListReader
{
    public sealed record CommonListSectionTuple(string Title, string BaseDirectory);
    private readonly Dictionary<string, CommonListSectionTuple> _sections;
    private readonly Dictionary<string, Dictionary<int, List<CommonListEntry>>> _entriesList;
    public CommonListReader(CommonListSectionTuple[] sections)
    {
        _sections = [];
        _entriesList = [];
        string lowertitle;
        foreach (CommonListSectionTuple section in sections)
        {
            lowertitle = $"{section.Title.ToLower().Trim()}:";
            _sections.TryAdd(lowertitle, section);
            _entriesList.TryAdd(lowertitle, []);
        }
    }
    public bool Read(string path, LogRegisterSystem log, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false)
    {
        FileReaderWithLog fReader = new(path, log);
        FileEnumeratorWithLog fileEnumerator = (FileEnumeratorWithLog)fReader.GetEnumerator()!;
        if(!fReader.SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, true, [.. _entriesList.Keys]))
            return false;

        CommonListParsingContext ctx;

        foreach (var section in enumerators)
        {
            ctx = new(section.Value, _sections[section.Key], maxID, allowVariables);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                if (!ctx.ProcessEntry())
                    return false;
            }
            _entriesList[section.Key] = ctx.GetEntries();
        }
        return true;
    }
    public IEnumerable<CommonListEntry> GetEntries()
    {
        List<CommonListEntry> entries = [];
        foreach (var entry in _entriesList)
        {
            entries.AddRange(entry.Value.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));
        }
        return entries;
    }
}
