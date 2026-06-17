using FormatLibrary;
using FormatLibrary.Entries;
using Newtonsoft.Json;
using SMWHubValidations.StateVariableValidations;
using Validations;

namespace FormatReadLibrary.Readers;

public static partial class NormalSpriteCFGReader
{
    public static ValidationResult Reader(string path, out NormalSpriteConfigEntry? config)
    {
        string content = FormatCleaner.CleanFileContent(path);
        switch(Path.GetExtension(path))
        {
            case ".cfg":
                return readCFG(content, out config);
            case ".json":
                return readJson(content, out config);
            default:
                config = null;
                return new();
        }
    }
    private static ValidationResult readCFG(string content, out NormalSpriteConfigEntry? config)
    {
        ValidationResult result = new();
        string[] lines = content.Split('\n');
        config = null;
        return result;
    }
    private static ValidationResult readJson(string content, out NormalSpriteConfigEntry? config)
    {
        ValidationResult result = new();
        try
        {
            config = JsonConvert.DeserializeObject<NormalSpriteConfigEntry>(content)!;
        }
        catch(Exception ex)
        {
            config = null;
            result.AddError(StateVariableMessageTypeKeys.RAW_SYNTAX_ERROR_MESSAGE, new()
            {
                { "message", ex.Message }
            });
        }
        return result;
    }
}
