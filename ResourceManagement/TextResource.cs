using ResourceManagement.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ResourceManagement;

public class TextResource : ITextResource
{
    public string Name { get; }
    public string? FilePath { get; private set; }
    public IResourceType Type { get; }
    public string Content { get; private set; }
    public TextResource(string name, IResourceType type, string content)
    {
        Name = name;
        Type = type;
        Content = content;
    }
    public TextResource(string filepath, IResourceType type)
    {
        Name = Path.GetFileNameWithoutExtension(filepath);
        Type = type;
        SetContentFromFile(filepath);
    }
    [MemberNotNull(nameof(FilePath))]
    [MemberNotNull(nameof(Content))]
    public void SetContentFromFile(string filepath)
    {
        if (!File.Exists(filepath))
            throw new FileNotFoundException(nameof(filepath), filepath);
        FilePath = filepath;
        Content = File.ReadAllText(filepath);
    }
}
