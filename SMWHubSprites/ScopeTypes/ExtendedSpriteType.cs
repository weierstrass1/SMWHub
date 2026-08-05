using SMWHubASMCodeLibrary;

namespace SMWHubSprites.ScopeTypes;

public class ExtendedSpriteType : IScopeType<ExtendedSpriteType>
{
    public bool AllowsSharedResources { get; } = true;
    public string Name { get; } = "Extendeds";
    public string? DefaultParentName { get; } = "Sprites";
    private static ExtendedSpriteType _instance { get; } = new();
    public static IScopeType GetInstance()
    {
        return _instance;
    }
}
