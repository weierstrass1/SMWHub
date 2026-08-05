using SMWHubASMCodeLibrary;

namespace SMWHubSprites.ScopeTypes;

public class ClusterSpriteType : IScopeType<ClusterSpriteType>
{
    public bool AllowsSharedResources { get; } = true;
    public string Name { get; } = "Clusters";
    public string? DefaultParentName { get; } = "Sprites";
    private static ClusterSpriteType _instance { get; } = new();
    public static IScopeType GetInstance()
    {
        return _instance;
    }
}
