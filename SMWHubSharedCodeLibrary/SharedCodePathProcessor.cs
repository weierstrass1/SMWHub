namespace SMWHubSharedCodeLibrary;

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
        new("./",SharedCodeScopeType.Global),
        new( "Sprites",SharedCodeScopeType.Sprite),
        new(Path.Combine("Sprites","Sprites"),SharedCodeScopeType.NormalSprite),
        new(Path.Combine("Sprites","Cluster"),SharedCodeScopeType.ClusterSprite),
        new(Path.Combine("Sprites","Extended"),SharedCodeScopeType.ExtendedSprite),
        new( "OverworldSprites",SharedCodeScopeType.OverworldSprite),
        new("UberASM",SharedCodeScopeType.UberASM),
        new(Path.Combine("UberASM","Level"),SharedCodeScopeType.LevelASM),
        new(Path.Combine("UberASM","Gamemode"),SharedCodeScopeType.GamemodeASM),
        new(Path.Combine("UberASM","Overworld"),SharedCodeScopeType.OverworldASM),
        new("Blocks",SharedCodeScopeType.Block),
        new("Patches",SharedCodeScopeType.Patch)
    ];
    public static SharedCode[] FindSharedCodes()
    {
        List<SharedCode> files = [];
        foreach(var scope in _sharedCodeRoots)
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
