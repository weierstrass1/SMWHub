namespace SMWHubSharedCodeLibrary;

public enum CodeType
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
    Player
}
public sealed class SharedCodeScope(string directoryPath, CodeType type)
{
    public readonly string DirectoryPath = directoryPath;
    public readonly CodeType Type = type;
}

