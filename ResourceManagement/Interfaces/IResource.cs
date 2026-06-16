namespace ResourceManagement.Interfaces;

public interface IResource
{
    public string Name { get; }
    public string? FilePath { get; }
    public IResourceType Type { get; }
}
