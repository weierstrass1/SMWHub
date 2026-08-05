using FormatLibrary.Entries;
using FormatLibrary.Interfaces;
using FormatReadLibrary.LineContexts;
using OneOf;
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
    public IEnumerable<OneOf<ValidationResult, (string, CommonListEntry)>> Read(string path, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false)
    {
        FileLineReader fReader = new(path);

        yield return fReader.SplitBySections(out Dictionary<string, FileLineEnumerator> enumerators, true, [.. _entriesList.Keys]);

        CommonListParsingContext ctx;
        ValidationResult result;

        foreach (var section in enumerators)
        {
            ctx = new((FileEnumeratorLineContext)section.Value, _sections[section.Key], maxID, allowVariables, allowMultiIDs);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                result = ctx.ProcessEntry();
                if (!result.IsValid)
                    yield return result;
            }
            foreach (var entry in ctx
                .GetEntries()
                .Select(e => (section.Key, e)))
                yield return entry;
        }
    }
}
