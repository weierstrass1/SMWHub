using FormatLibrary.Entries;
namespace SMWHubASMCodeLibrary;

public enum ScopeType
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
    public static Code[] GetNormalSprites(CodeScopeContainer scopes, IEnumerable<NormalSpriteConfigEntry> list)
    {
        return [.. list.Select(sce => new Code(sce.Filepath, CodeType.ASM, scopes[ScopeType.NormalSprite]))];
    }
}
