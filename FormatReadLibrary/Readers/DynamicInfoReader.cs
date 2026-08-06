using FormatLibrary;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using SMWHubEnumerators;
using SMWHubValidations.StateVariableValidations;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private static readonly string[] _sections = [
        "posesgraphics:",
        "palettes:",
        "resources:",
        "poseschunkssizes:",
        "numberof16x16tilesperpose:"
        ];
    public static ValidationResult Read(string path, string baseDirectory, out DynamicInfo? dynamicInfo)
    {
        var oneOfs = FileSection.GetSectionsFromFile(path, _sections).ToList();

        var val = oneOfs.FirstOrDefault(s => s.IsT0);
        if(val.IsT0)
        {
            dynamicInfo = null;
            return val.AsT0;
        }

        ParsingContext ctx;
        dynamicInfo = new();

        var sections = oneOfs
            .Select(s => s.AsT1);

        var sortedSections = sections
            .Where(s => s.Name != "poseschunkssizes:" && s.Name != "numberof16x16tilesperpose:")
            .ToList();

        sortedSections.AddRange(sections
            .Where(s => s.Name == "poseschunkssizes:" || s.Name == "numberof16x16tilesperpose:"));

        FileLineEnumerator fle;

        ValidationResult result = new();

        foreach (var section in sortedSections)
        {
            fle = section.GetEnumerator();
            ctx = createContext(section.Name, dynamicInfo, baseDirectory, (FileEnumeratorLineContext) fle);
            while (fle.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(fle.Current))
                    continue;
                result.Merge(ctx.ProcessEntry());
            }
        }

        return result;
    }
    private static ValidationResult validateIfUseBothFormats(FileLineReader fReader, Dictionary<string, FileLineEnumerator> enumerators)
    {
        ValidationResult result = new();
        if (enumerators.TryGetValue("poseschunkssizes:", out FileLineEnumerator? legacyFormat) &&
            enumerators.TryGetValue("numberof16x16tilesperpose:", out FileLineEnumerator? currentFormat))
        {
            int i = Math.Max(legacyFormat.LineIndex, currentFormat.LineIndex);
            result.Context = new(fReader.FilePath, i, fReader[i]);
            result.AddError(StateVariableMessageTypeKeys.BOTH_DYNAMIC_INFO_FORMATS);
            return result;
        }
        return result;
    }
    private static ParsingContext createContext(string section, DynamicInfo dynamicInfo, string baseDirectory, FileEnumeratorLineContext context)
    {
        return section switch
        {
            "posesgraphics:" => new DynamicInfoResourceListParsingContext(context, baseDirectory, DynamicInfoSection.PosesGraphics)
            { DynamicInfo = dynamicInfo },
            "palettes:" => new DynamicInfoResourceListParsingContext(context, baseDirectory, DynamicInfoSection.Palettes)
            { DynamicInfo = dynamicInfo },
            "resources:" => new DynamicInfoResourceListParsingContext(context, baseDirectory, DynamicInfoSection.Resources)
            { DynamicInfo = dynamicInfo },
            "poseschunkssizes:" => new DynamicInfoLegacyFormatParsingContext(context)
            { DynamicInfo = dynamicInfo },
            "numberof16x16tilesperpose:" => new DynamicInfoCurrentFormatParsingContext(context)
            { DynamicInfo = dynamicInfo },
            _ => throw new Exception($"Unknown section type: {section}")
        };
    }
    private enum DynamicInfoSection
    {
        PosesGraphics,
        Palettes,
        Resources,
        PosesChunkSizes,
        NumberOf16x16TilesPerPose
    }
}
