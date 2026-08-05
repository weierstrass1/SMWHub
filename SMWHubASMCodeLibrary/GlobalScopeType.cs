namespace SMWHubASMCodeLibrary;

public class GlobalScopeType : IScopeType<GlobalScopeType>
{
    public bool AllowsSharedResources { get; } = true;
    private static IScopeType _instance { get; } = new GlobalScopeType();
    public string Name { get; } = "Global";
    public string? DefaultParentName { get; } = null;
    private GlobalScopeType()
    {
    }
    public static IScopeType GetInstance()
    {
        return _instance;
    }
}
