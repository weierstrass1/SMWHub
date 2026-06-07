using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers.ParsingContexts;
using LogRegister;
using SMWHubEnumerators;
using SMWHubValidations;
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
    public static bool Read(string path, out DynamicInfo? dynamicInfo)
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
            ctx = createContext(section.Key, dynamicInfo, baseDirectory, section.Value);
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
            result.AddError(ValidatorMessagetypeKeys.BOTH_DYNAMIC_INFO_FORMATS);
            return result;
        }
        return result;
    }
    private static ParsingContext createContext(string section, DynamicInfo dynamicInfo, string baseDirectory, FileEnumerator fileEnumerator)
    {
        return section switch
        {
            "posesgraphics:" => new DynamicInfoResourceListParsingContext(fileEnumerator, baseDirectory, DynamicInfoSection.PosesGraphics)
            { DynamicInfo = dynamicInfo },
            "palettes:" => new DynamicInfoResourceListParsingContext(fileEnumerator, baseDirectory, DynamicInfoSection.Palettes)
            { DynamicInfo = dynamicInfo },
            "resources:" => new DynamicInfoResourceListParsingContext(fileEnumerator, baseDirectory, DynamicInfoSection.Resources)
            { DynamicInfo = dynamicInfo },
            "poseschunkssizes:" => new DynamicInfoLegacyFormatParsingContext(fileEnumerator)
            { DynamicInfo = dynamicInfo },
            "numberof16x16tilesperpose:" => new DynamicInfoCurrentFormatParsingContext(fileEnumerator)
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
