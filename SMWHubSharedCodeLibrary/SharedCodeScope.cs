namespace SMWHubSharedCodeLibrary;

public enum SharedCodeScopeType
{
    Global,
    Sprite,
    NormalSprite,
    ClusterSprite,
    ExtendedSprite,
    OverworldSprite,
    Generator,
    MinorExtendedSprite,
    SmokeSprite,
    UberASM,
    LevelASM,
    GamemodeASM,
    OverworldASM,
    Block,
    Patch,
    Player,

}
public sealed class SharedCodeScope(string directoryPath, SharedCodeScopeType type)
{
    public readonly string DirectoryPath = directoryPath;
    public readonly SharedCodeScopeType Type = type;
}

