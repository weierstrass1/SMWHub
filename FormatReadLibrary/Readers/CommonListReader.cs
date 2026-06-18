using FormatLibrary.Entries;
using FormatReadLibrary.Interfaces;
using FormatReadLibrary.LineContexts;
using SMWHubEnumerators;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class CommonListReader
{
    private readonly Dictionary<string, ICommonListCategory> _sections;
    private readonly Dictionary<string, Dictionary<int, List<CommonListEntry>>> _entriesList;
    public CommonListReader(ICommonListCategory[] sections)
    {
        _sections = [];
        _entriesList = [];
        string lowertitle;
        foreach (ICommonListCategory section in sections)
        {
            lowertitle = $"{section.Title.ToLower().Trim()}:";
            _sections.TryAdd(lowertitle, section);
            _entriesList.TryAdd(lowertitle, []);
        }
    }
    public ValidationResult Read(string path, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false)
    {
        FileReader fReader = new(path);

        ValidationResult result = fReader.SplitBySections(out Dictionary<string, FileEnumerator> enumerators, true, [.. _entriesList.Keys]);

        CommonListParsingContext ctx;

        foreach (var section in enumerators)
        {
            ctx = new((FileEnumeratorLineContext)section.Value, _sections[section.Key], maxID, allowVariables, allowMultiIDs);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                result.Merge(ctx.ProcessEntry());
            }
            _entriesList[section.Key] = ctx.GetEntries();
        }
        return result;
    }
    public IEnumerable<CommonListEntry> GetEntries()
    {
        List<CommonListEntry> entries = [];
        IEnumerable<List<CommonListEntry>> entriesListOfLists;
        foreach (var entry in _entriesList)
        {
            entriesListOfLists = entry.Value
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value);
            foreach (var entriesList in entriesListOfLists)
                entries.AddRange(entriesList);
        }
        return entries;
    }
}
