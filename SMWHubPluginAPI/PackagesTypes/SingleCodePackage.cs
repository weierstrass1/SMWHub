using SMWHubASMCodeLibrary;

namespace SMWHubPluginAPI.PackagesTypes;

public class SingleCodePackage(string filepath, CodeType type, CodeScope scope) : IPackage
{
    public IPatchPlugin? PatchPlugin { get; init; } = null;
    public required IResourcePlugin OriginPlugin { get; init; }
    public Priority Priority { get; init; } = 0;
    public CodeType Type { get; } = type;
    public CodeScope Scope { get; } = scope;
    public string PackagePath { get; } = filepath;
    public IEnumerable<string> GetFiles()
    {
        return [Path.Combine(Scope.SourceDirectoryPath, PackagePath)];
    }
    public IPackage? GetSubPackageFromInternalFile(string filepath)
    {
        if (filepath != PackagePath)
            return null;
        return this;
    }
    public long GetSize()
    {
        return new FileInfo(PackagePath).Length;
    }
    public string ObtainHashCode()
    {
        return IPackage.GetHashFromFile(PackagePath);
    }
}
