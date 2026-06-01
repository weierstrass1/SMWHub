namespace ResourceManagement;

public class Resource(int id, string name, IResourceType type, byte[] data)
{
    public int ID { get; private set; } = id;
    public string Name { get; private set; } = name;
    public IResourceType Type { get; private set; } = type;
    public virtual byte[] Data { get; protected set; } = data;
    public int Length => Data.Length;

    public override string ToString()
    {
        return $"({ID}, {Name}, {Type}, {Length})";
    }
}
