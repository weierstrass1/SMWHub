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
    public IEnumerable<OneOf<ValidationResult, (string, CommonListEntry)>> Read(string path, string? defaultSection = null, int maxID = 255, bool allowVariables = false, bool allowMultiIDs = false)
    {  
        CommonListParsingContext ctx;
        ValidationResult result;
        FileLineEnumerator fle;
        FileSection section;

        foreach (var sec in FileSection.GetSectionsFromFile(path, _sections.Keys.ToHashSet(), defaultSection))
        {
            if (sec.IsT0)
            {
                yield return sec.AsT0;
                yield break;
            }
            section = sec.AsT1;
            fle = section.GetEnumerator();
            ctx = new((FileEnumeratorLineContext)fle, _sections[section.Name], maxID, allowVariables, allowMultiIDs);
            while (fle.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(fle.Current))
                    continue;
                result = ctx.ProcessEntry();
                if (!result.IsValid)
                    yield return result;
            }
            foreach (var entry in ctx
                .GetEntries()
                .Select(e => (section.Name, e)))
                yield return entry;
        }
    }
}
