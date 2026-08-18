namespace SMWHubASMCodeLibrary;
public interface IScopeType
{
    public string Name { get; }
    public string? DefaultParentName { get; }
    public bool AllowsSharedResources { get; }
    public static IScopeType GetInstance<T>() where T : IScopeType<T>
    {
        return T.GetInstance();
    }
}
public interface IScopeType<T> : IScopeType where T : IScopeType
{
    public static abstract IScopeType GetInstance();
}
public interface ICustomRoutineDefinition
{
    public string Define(Code code); 
}