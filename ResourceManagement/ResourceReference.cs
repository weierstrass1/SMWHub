using ResourceManagement.Interfaces;

namespace ResourceManagement;

public sealed class ResourceReference(int id, int bufferID, int position, IResource resource)
{
    public int ID { get; private set; } = id;
    public int BufferID { get; private set; } = bufferID;
    public int Position { get; set; } = position;
    public IResource Resource { get; private set; } = resource;

    public override string ToString()
        => $"({ID}, {Resource.Type}, {BufferID}, {Position})";
}
