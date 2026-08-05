using System.ComponentModel;
namespace SMWHubASMCodeLibrary;

public enum ScopeType
{
    [Description("Global")]
    Global,
    [Description("Sprite")]
    Sprite,
    [Description("Sprites")]
    NormalSprite,
    [Description("Clusters")]
    ClusterSprite,
    [Description("Extendeds")]
    ExtendedSprite,
    [Description("OverworldSprites")]
    OverworldSprite,
    [Description("Generators")]
    Generator,
    [Description("MinorExtendeds")]
    MinorExtendedSprite,
    [Description("Smokes")]
    SmokeSprite,
    [Description("UberASM")]
    UberASM,
    [Description("Level")]
    LevelASM,
    [Description("Gamemode")]
    GamemodeASM,
    [Description("OverworldASM")]
    OverworldASM,
    [Description("Blocks")]
    Block,
    [Description("Patches")]
    Patch,
    [Description("Players")]
    Player
}
public static class ASMCodePathProcessor
{
}
