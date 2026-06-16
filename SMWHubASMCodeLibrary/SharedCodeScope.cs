namespace SMWHubASMCodeLibrary;

public sealed class SharedCodeScope(string directoryPath, CodeType type)
{
    public readonly string DirectoryPath = directoryPath;
    public readonly CodeType Type = type;
}
