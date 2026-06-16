namespace SMWHubASMCodeLibrary;

public enum SharedCodeTypes
{
    Macros,
    Defines,
    Routines
};
public class SharedCodePathProcessor
{
    public const string SHARED_CODE_DIRECTORY = "_SharedCode";
    private static readonly IReadOnlyList<SharedCodeScope> _sharedCodeRoots =
    [
        new("./",CodeType.Global),
        new( "Sprites",CodeType.Sprite),
        new(Path.Combine("Sprites","Sprites"),CodeType.NormalSprite),
        new(Path.Combine("Sprites","Cluster"),CodeType.ClusterSprite),
        new(Path.Combine("Sprites","Extended"),CodeType.ExtendedSprite),
        new( "OverworldSprites",CodeType.OverworldSprite),
        new("UberASM",CodeType.UberASM),
        new(Path.Combine("UberASM","Level"),CodeType.LevelASM),
        new(Path.Combine("UberASM","Gamemode"),CodeType.GamemodeASM),
        new(Path.Combine("UberASM","Overworld"),CodeType.OverworldASM),
        new("Blocks",CodeType.Block),
        new("Patches",CodeType.Patch)
    ];
    public static SharedCode[] FindSharedCodes()
    {
        List<SharedCode> files = [];
        foreach (var scope in _sharedCodeRoots)
            files.AddRange(getSharedCodeTypesFiles(scope));
        return [.. files];
    }
    private static SharedCode[] getSharedCodeTypesFiles(SharedCodeScope scope)
    {
        List<SharedCode> files = [];
        foreach (var type in Enum.GetValues<SharedCodeTypes>())
            files.AddRange(getFiles(scope, type));
        return [.. files];
    }
    private static IEnumerable<SharedCode> getFiles(SharedCodeScope scope, SharedCodeTypes type)
    {
        string tDirectory = Path.Combine(scope.DirectoryPath, SHARED_CODE_DIRECTORY, type.ToString());
        var files = ensureDirectoryAndGetFiles(tDirectory);
        return files.Select(f => new SharedCode(f, type, scope));
    }
    private static IEnumerable<string> ensureDirectoryAndGetFiles(string tDirectory)
    {
        Directory.CreateDirectory(tDirectory);
        string[] files = Directory.GetFiles(tDirectory, "*.asm", SearchOption.AllDirectories);
        return files.Select(fp => Path.GetRelativePath("./", fp));
    }
}
