using SMWHubASMCodeLibrary;

namespace SMWHubSprites.ScopeTypes;

public class SpriteType : IScopeType<SpriteType>, ICustomRoutineDefinition
{
    public bool AllowsSharedResources { get; } = true;
    public string Name { get; } = "Sprites";
    public string? DefaultParentName { get; } = "Global";
    private static SpriteType _instance { get; } = new();
    public static IScopeType GetInstance()
    {
        return _instance;
    }
    public string Define(Code code)
    {
        return SpriteResourcePlugin.CustomRoutineDefinition(code);
    }
}
