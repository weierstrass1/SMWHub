using SMWHubASMCodeLibrary;

namespace SMWHubSprites.ScopeTypes;

public class NormalSpriteType : IScopeType<NormalSpriteType>, ICustomRoutineDefinition
{
    public bool AllowsSharedResources { get; } = true;
    public string Name { get; } = "NormalSprites";
    public string? DefaultParentName { get; } = "Sprites";
    private static NormalSpriteType _instance { get; } = new();
    public static IScopeType GetInstance()
    {
        return _instance;
    }
    public string Define(Code code)
    {
        return SpriteResourcePlugin.CustomRoutineDefinition(code);
    }
}
