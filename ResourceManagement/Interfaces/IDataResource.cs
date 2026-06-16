namespace ResourceManagement.Interfaces;

public interface IDataResource : IResource
{
    public byte[] Data { get; }
    public int Length => Data.Length;
}
