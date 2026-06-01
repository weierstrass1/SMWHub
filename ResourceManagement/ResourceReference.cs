namespace ResourceManagement;

public sealed class ResourceReference(int bufferID, int position, Resource resource)
{
    public int BufferID { get; private set; } = bufferID;
    public int Position { get; set; } = position;
    public Resource Resource { get; private set; } = resource;

    public override string ToString()
        => $"({Resource.ID}, {Resource.Type}, {BufferID}, {Position})";
}
