using FormatLibrary;
using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using Newtonsoft.Json;
using SMWHubEnumerators;
using SMWHubSprites.CommonListCategories;
using SMWHubValidations.StateVariableValidations;
using Validations;
using ZWXStateMachine;

namespace SMWHubSprites.Formats.Readers;

public static partial class SpriteCFGReader
{
    public static ValidationResult Read(IEnumerable<CommonListEntry> entries, out List<SpriteConfigEntry> cfgs)
    {
        var cfgsPaths = entries
            .Where(e =>
            {
                if (e.Category is not NormalSprite)
                    return false;
                string ext = Path.GetExtension(e.Paths[0].Path);
                return ext == ".cfg" || ext == ".json";
            });

        ValidationResult result = new();

        cfgs = []; 
        foreach (var p in cfgsPaths)
        {
            result.Merge(Read(p.Paths[0].Path, out SpriteConfigEntry? cfg));
            if (cfg == null)
                continue;
            cfg.ID = p.ID;
            cfgs.Add(cfg);
        }
        return result;
    }
    public static ValidationResult Read(string path, out SpriteConfigEntry? config)
    {
        ValidationResult result;
        switch (Path.GetExtension(path))
        {
            case ".cfg":
                result = readCFG(path, out config);
                break;
            case ".json":
                result = readJson(path, out config);
                break;
            default:
                config = null;
                result = new();
                break;
        }
        if (config != null)
            config.CFGPath = path;
        return result;
    }
    private static ValidationResult readCFG(string path, out SpriteConfigEntry? config)
    {
        ValidationResult result = new();
        FileLineReader freader = new(path);
        FileLineEnumerator en = (FileLineEnumerator)freader.GetEnumerator();

        SpriteCFGParsingContext ctx = new(new FileEnumeratorLineContext(en));

        while (en.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(en.Current))
                continue;
            result.Merge(ctx.ProcessEntry());
        }

        StateData state = ctx.StateData;

        config = new()
        {
            Type = state.Get<int>("Type"),
            ActLike = state.Get<byte>("ActLike"),
            Tweak1656 = state.Get<byte>("$1656")!,
            Tweak1662 = state.Get<byte>("$1662")!,
            Tweak166E = state.Get<byte>("$166E")!,
            Tweak167A = state.Get<byte>("$167A")!,
            Tweak1686 = state.Get<byte>("$1686")!,
            Tweak190F = state.Get<byte>("$190F")!,
            ExtraPropertyByte1 = state.Get<byte>("Prop1"),
            ExtraPropertyByte2 = state.Get<byte>("Prop2"),
            Filepath = state.Get<string>("FilePath")!,
            ExtraBytesWithClearExtraBit = state.Get<int>("CleanEBAmount"),
            ExtraBytesWithSetExtraBit = state.Get<int>("SetEBAmount")
        };
        return result;
    }
    private static ValidationResult readJson(string path, out SpriteConfigEntry? config)
    {
        string content = FormatCleaner.CleanFileContent(path);
        ValidationResult result = new();
        try
        {
            config = JsonConvert.DeserializeObject<SpriteConfigEntry>(content)!;
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
