using FormatLibrary;
using SMWHubValidations.StateVariableValidations;
using System.Text.RegularExpressions;
using Validations;

namespace SMWHubASMCodeLibrary;

public enum EmbeddedFileType
{
    DynamicInfo,
    DrawInfo,
    HitboxInfo,
    CFG,
    JsonCFG,
    Binary,
    PaletteEffect,
    HDMAEffect,
    Palette,
    CustomProperty
}
public class EmbeddedFile
{
    public required string OriginFile { get; init; }
    public required string Name { get; init; }
    public required EmbeddedFileType Type { get; init; }
    public required int StartLine { get; init; }
    public required int EndLine { get; init; }

    public static ValidationResult GetEmbeddedFileFromASM(string path, out EmbeddedFile[] embeddedFiles)
    {
        ValidationResult result = new();
        result.Context = new(path, 0, "");
        if (!File.Exists(path))
        {
            embeddedFiles = [];
            result.AddError(StateVariableMessageTypeKeys.FILE_NOT_FOUND);
            return result;
        }
        string content = FormatCleaner.CleanFileContent(path);
        string[] lines = content.Split('\n');
        string pattern = $@"#embedded (?<type>({string.Join('|', Enum.GetNames<EmbeddedFileType>())}))( (?<name>[a-zA-Z][a-zA-Z_0-9]*))?";
        Regex r = new(pattern);
        int start = -1, end = 0;
        Match m;
        Dictionary<(EmbeddedFileType,string), EmbeddedFile> files = [];
        string defaultName = Path.GetFileNameWithoutExtension(path);
        string name = defaultName;
        EmbeddedFileType type = default;
        for (int i = 0; i < lines.Length; i++) 
        {
            m = r.Match(lines[i]);
            if (start < end && m.Success)
            {
                start = i;
                type = Enum.Parse<EmbeddedFileType>(m.Groups["type"].ToString());
                name = m.Groups["name"].Success ? m.Groups["name"].ToString() : defaultName;
                
                continue;
            }
            if (lines[i] == "#end embedded" && start > end)
            {
                end = i;
                files.Add((type, name), new()
                {
                    Name = name,
                    Type = type,
                    StartLine = start,
                    EndLine = end,
                    OriginFile = path
                });
            }
        }
        embeddedFiles = [.. files.Values];
        return result;
    }
}
