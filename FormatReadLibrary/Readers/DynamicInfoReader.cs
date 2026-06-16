using FormatReadLibrary.Infos;
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
    public static ValidationResult Read(string path, out DynamicInfo? dynamicInfo)
    {
        string content = File.ReadAllText(path);
        return Read(Path.GetFileNameWithoutExtension(path), content, Path.GetDirectoryName(path)!, out dynamicInfo);
    }
    public static ValidationResult Read(string name, string dynamicInfoContent, string baseDirectory, out DynamicInfo? dynamicInfo)
    {
        FileReader fReader = new(name, dynamicInfoContent);

        ValidationResult result = fReader.SplitBySections(out Dictionary<string, FileEnumerator> enumerators, true, _sections);

        result.Merge(validateIfUseBothFormats(fReader, enumerators));

        ParsingContext ctx;
        dynamicInfo = new(Path.GetFileNameWithoutExtension(name));

        foreach (var section in enumerators)
        {
            ctx = createContext(section.Key, dynamicInfo, baseDirectory, (FileEnumeratorLineContext)section.Value);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                result.Merge(ctx.ProcessEntry());
            }
        }

        return result;
    }
    private static ValidationResult validateIfUseBothFormats(FileReader fReader, Dictionary<string, FileEnumerator> enumerators)
    {
        ValidationResult result = new();
        if (enumerators.TryGetValue("poseschunkssizes:", out FileEnumerator? legacyFormat) &&
            enumerators.TryGetValue("numberof16x16tilesperpose:", out FileEnumerator? currentFormat))
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
