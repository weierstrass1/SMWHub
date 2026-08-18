using SMWHubASMCodeLibrary;

namespace SMWHubSprites.ScopeTypes;

public class ClusterSpriteType : IScopeType<ClusterSpriteType>, ICustomRoutineDefinition
{
    public bool AllowsSharedResources { get; } = true;
    public string Name { get; } = "Clusters";
    public string? DefaultParentName { get; } = "Sprites";
    private static ClusterSpriteType _instance { get; } = new();
    public static IScopeType GetInstance()
    {
        return _instance;
    }
    public string Define(Code code)
    {
        return SpriteResourcePlugin.CustomRoutineDefinition(code);
    }
}
