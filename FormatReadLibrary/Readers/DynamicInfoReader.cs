using FormatReadLibrary.Infos;
using FormatReadLibrary.Logging.LoggingRegisters;
using FormatReadLibrary.Readers.Enumerators;
using FormatReadLibrary.Readers.ParsingContexts;
using LogRegister;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    public bool Read(string path, LogRegisterSystem log, out DynamicInfo? dynamicInfo)
    {
        string content = File.ReadAllText(path);
        return Read(Path.GetFileNameWithoutExtension(path), content, log, out dynamicInfo);
    }
    public bool Read(string name, string dynamicInfoContent, LogRegisterSystem log, out DynamicInfo? dynamicInfo)
    {
        FileReaderWithLog fReader = new(name, dynamicInfoContent, log);

        dynamicInfo = null;
        if (!fReader.SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, true,
            "posesgraphics:", "palettes:", "resources:", "poseschunkssizes:", "numberof16x16tilesperpose:"))
            return false;

        if (!validateIfUseBothFormats(name, log, fReader, enumerators))
            return false;

        ParsingContext ctx;
        dynamicInfo = new(Path.GetFileNameWithoutExtension(name));

        foreach (var section in enumerators)
        {
            ctx = createContext(section.Key, dynamicInfo, section.Value);
            while (section.Value.MoveNext())
            {
                if (string.IsNullOrWhiteSpace(section.Value.Current))
                    continue;
                if (!ctx.ProcessEntry())
                    return false;
            }
        }

        return true;
    }
    private static bool validateIfUseBothFormats(string name, LogRegisterSystem log, FileReaderWithLog fReader, Dictionary<string, FileEnumeratorWithLog> enumerators)
    {
        if (enumerators.TryGetValue("poseschunkssizes:", out FileEnumeratorWithLog? legacyFormat) &&
            enumerators.TryGetValue("numberof16x16tilesperpose:", out FileEnumeratorWithLog? currentFormat))
        {
            int i = Math.Max(legacyFormat.LineIndex, currentFormat.LineIndex);
            log.Add(new SyntaxError(i, name, fReader[i], $"Both 'poseschunkssizes:' and 'numberof16x16tilesperpose:' sections are present. You can't use legacy and current format at the same time."));
            return false;
        }
        return true;
    }
    private ParsingContext createContext(string section, DynamicInfo dynamicInfo, FileEnumeratorWithLog fileEnumerator)
    {
        return section switch
        {
            "posesgraphics:" => new DynamicInfoResourceListParsingContext(fileEnumerator, DynamicInfoSection.PosesGraphics)
            { DynamicInfo = dynamicInfo },
            "palettes:" => new DynamicInfoResourceListParsingContext(fileEnumerator, DynamicInfoSection.Palettes)
            { DynamicInfo = dynamicInfo },
            "resources:" => new DynamicInfoResourceListParsingContext(fileEnumerator, DynamicInfoSection.Resources)
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
