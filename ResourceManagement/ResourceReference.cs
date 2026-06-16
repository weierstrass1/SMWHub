using ResourceManagement.Interfaces;

namespace ResourceManagement;

public sealed class ResourceReference(int bufferID, int position, IResource resource)
{
    public int BufferID { get; private set; } = bufferID;
    public int Position { get; set; } = position;
    public IResource Resource { get; private set; } = resource;

    public override string ToString()
        => $"({Resource.ID}, {Resource.Type}, {BufferID}, {Position})";
}
