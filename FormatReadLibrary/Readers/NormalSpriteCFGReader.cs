using FormatLibrary;
using FormatLibrary.CommonListCategories;
using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using Newtonsoft.Json;
using SMWHubEnumerators;
using SMWHubValidations.StateVariableValidations;
using Validations;
using ZWXStateMachine;

namespace FormatReadLibrary.Readers;

public static partial class NormalSpriteCFGReader
{
    public static ValidationResult Read(IEnumerable<CommonListEntry> entries, out List<NormalSpriteConfigEntry> cfgs)
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
            result.Merge(Read(p.Paths[0].Path, out NormalSpriteConfigEntry? cfg));
            if (cfg == null)
                continue;
            cfg.ID = p.ID;
            cfg.CFGPath = p.Paths[0].Path;
            cfgs.Add(cfg);
        }
        return result;
    }
    public static ValidationResult Read(string path, out NormalSpriteConfigEntry? config)
    {
        switch(Path.GetExtension(path))
        {
            case ".cfg":
                return readCFG(path, out config);
            case ".json":
                return readJson(path, out config);
            default:
                config = null;
                return new();
        }
    }
    private static ValidationResult readCFG(string path, out NormalSpriteConfigEntry? config)
    {
        ValidationResult result = new();
        FileReader freader = new(path);
        FileEnumerator en = (FileEnumerator)freader.GetEnumerator();

        NormalSpriteCFGParsingContext ctx = new(new FileEnumeratorLineContext(en));

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
    private static ValidationResult readJson(string path, out NormalSpriteConfigEntry? config)
    {
        string content = FormatCleaner.CleanFileContent(path);
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
