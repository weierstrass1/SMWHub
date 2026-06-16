using ResourceManagement.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ResourceManagement;

public class DataResource : IDataResource
{
    public string Name { get; }
    public string? FilePath { get; private set; }
    public IResourceType Type { get; }
    public byte[] Data { get; protected set; }
    public DataResource(string name, IResourceType type, byte[] data) 
    {
        Name = name;
        Type = type;
        Data = data;
    }
    public DataResource(string filepath, IResourceType type)
    {
        Name = Path.GetFileNameWithoutExtension(filepath);
        Type = type;
        SetDataFromFile(filepath);
    }
    [MemberNotNull(nameof(FilePath))]
    [MemberNotNull(nameof(Data))]
    public void SetDataFromFile(string filepath)
    {
        if (!File.Exists(filepath))
            throw new FileNotFoundException(nameof(filepath), filepath);
        FilePath = filepath;
        Data = File.ReadAllBytes(filepath);
    }
}
