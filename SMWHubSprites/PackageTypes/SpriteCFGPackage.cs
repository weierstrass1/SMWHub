using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using SMWHubPluginAPI.PackagesTypes;
using SMWHubSprites.Formats;

namespace SMWHubSprites.PackageTypes;

public class SpriteCFGPackage(SpriteConfigEntry cfg, CodeType type, CodeScope scope) : IPackage
{
    public CodeType Type { get; } = type;
    public CodeScope Scope { get; } = scope;
    public string PackagePath { get; } = cfg.CFGPath!;
    private readonly SpriteConfigEntry _cfg = cfg;
    public IEnumerable<string> GetFiles()
    {
        yield return Path.Combine(Scope.SourceDirectoryPath, PackagePath);
        yield return Path.Combine(Scope.SourceDirectoryPath, _cfg.Filepath);
    }
    public IPackage? GetSubPackageFromInternalFile(string filepath)
    {
        if (filepath == PackagePath)
            return this;
        if (filepath == _cfg.Filepath)
            return new SingleCodePackage(_cfg.Filepath, Type, Scope);
        return null;
    }
    public long GetSize()
    {
        return GetFiles().Sum(f => new FileInfo(f).Length);
    }
    public string ObtainHashCode()
    {
        return IPackage.GetHashFromMultiplesFiles(GetFiles());
    }
}
