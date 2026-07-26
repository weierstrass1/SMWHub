using FormatLibrary.Entries;
using SMWHubASMCodeLibrary;

namespace SMWHubInstallation.PackagesTypes
{
    public class SpriteCFGPackage(NormalSpriteConfigEntry cfg, CodeScope scope) : IPackage
    {
        public CodeScope Scope { get; } = scope;
        public string PackagePath { get; } = cfg.CFGPath!;
        private readonly NormalSpriteConfigEntry _cfg = cfg;
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
                return new SingleCodePackage(_cfg.Filepath, Scope);
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
}
