using SMWHubASMCodeLibrary;

namespace SMWHubInstallation.PackagesTypes
{
    public class SingleCodePackage(string filepath, CodeScope scope) : IPackage
    {
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
}
