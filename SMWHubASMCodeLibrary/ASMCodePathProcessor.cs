using FormatLibrary.Entries;
namespace SMWHubASMCodeLibrary;
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
public static class ASMCodePathProcessor
{
    public static ASMCode[] GetNormalSprites(IEnumerable<NormalSpriteConfigEntry> list)
    {
        return [.. list.Select(sce => new ASMCode(sce.Filepath, CodeType.NormalSprite))];
    }
}
