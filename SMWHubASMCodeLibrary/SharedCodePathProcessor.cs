using SMWHubASMCodeLibrary.DTO;

namespace SMWHubASMCodeLibrary;

public enum CodeType
{
    Macros,
    Defines,
    Routines,
    ASM
};
public class SharedCodePathProcessor
{
    public const string SHARED_CODE_DIRECTORY = "_SharedCode";
    private readonly IReadOnlyDictionary<ScopeType, CodeScope> _sharedCodeRoots;
    public SharedCodePathProcessor(string path)
    {
        FolderContainer folders = FolderContainer.GetFromJson(path);
        SpriteFolderContainer sprDir = folders.SpritesFolders;
        UberasmFolderContainer uasmDir = folders.UberasmFolder;
        CodeScope main = new(folders.Main, ScopeType.Global);
        CodeScope sprite = new(sprDir.Main, ScopeType.Sprite, main);
        CodeScope uberasm = new(uasmDir.Main, ScopeType.UberASM, main);
        _sharedCodeRoots = new Dictionary<ScopeType, CodeScope>()
        {
            { ScopeType.Global, main},
            { ScopeType.Sprite, sprite },
            { ScopeType.NormalSprite, new(Path.Combine(sprDir.Main, sprDir.NormalSpritesFolder), ScopeType.NormalSprite, sprite) },
            { ScopeType.ClusterSprite,new(Path.Combine(sprDir.Main, sprDir.ClusterSpritesFolder), ScopeType.ClusterSprite, sprite) },
            { ScopeType.ExtendedSprite, new(Path.Combine(sprDir.Main, sprDir.ExtendedSpritesFolder), ScopeType.ExtendedSprite, sprite) },
            { ScopeType.UberASM, uberasm },
            { ScopeType.LevelASM, new(Path.Combine(uasmDir.Main, uasmDir.LevelFolder), ScopeType.LevelASM, uberasm) },
            { ScopeType.OverworldASM, new(Path.Combine(uasmDir.Main, uasmDir.OverworldFolder), ScopeType.OverworldASM, uberasm) },
            { ScopeType.GamemodeASM, new(Path.Combine(uasmDir.Main, uasmDir.GameModeFolder), ScopeType.GamemodeASM, uberasm) },
            { ScopeType.OverworldSprite, new(folders.OverworldSpritesFolder, ScopeType.OverworldSprite, main) },
            { ScopeType.Block, new(folders.BlocksFolder, ScopeType.Block, main) },
            { ScopeType.Patch, new(folders.PatchesFolder, ScopeType.Patch, main) }
        }.AsReadOnly();
    }
    public CodeScope GetScope(ScopeType type)
    {
        return _sharedCodeRoots[type];
    }
    public Code[] FindSharedCodes()
    {
        List<Code> files = [];
        foreach (var scope in _sharedCodeRoots.Values)
            files.AddRange(getSharedCodeTypesFiles(scope));
        return [.. files];
    }
    private Code[] getSharedCodeTypesFiles(CodeScope scope)
    {
        List<Code> files = [];
        foreach (var type in Enum.GetValues<CodeType>().Where(t => t != CodeType.ASM))
            files.AddRange(getFiles(scope, type));
        return [.. files];
    }
    private IEnumerable<Code> getFiles(CodeScope scope, CodeType type)
    {
        string tDirectory = Path.Combine(scope.SourceDirectoryPath, SHARED_CODE_DIRECTORY, type.ToString());
        var files = ensureDirectoryAndGetFiles(tDirectory);
        return files.Select(f => new Code(f, type, scope));
    }
    private IEnumerable<string> ensureDirectoryAndGetFiles(string tDirectory)
    {
        Directory.CreateDirectory(tDirectory);
        string[] files = Directory.GetFiles(tDirectory, "*.asm", SearchOption.AllDirectories);
        return files.Select(fp => Path.GetRelativePath("./", fp));
    }
}
