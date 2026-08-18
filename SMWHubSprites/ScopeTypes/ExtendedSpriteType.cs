using SMWHubASMCodeLibrary;

namespace SMWHubSprites.ScopeTypes;

public class ExtendedSpriteType : IScopeType<ExtendedSpriteType>, ICustomRoutineDefinition
{
    public bool AllowsSharedResources { get; } = true;
    public string Name { get; } = "Extendeds";
    public string? DefaultParentName { get; } = "Sprites";
    private static ExtendedSpriteType _instance { get; } = new();
    public static IScopeType GetInstance()
    {
        return _instance;
    }
    public string Define(Code code)
    {
        return SpriteResourcePlugin.CustomRoutineDefinition(code);
    }
}
