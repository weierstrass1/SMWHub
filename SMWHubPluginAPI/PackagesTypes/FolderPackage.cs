using SMWHubASMCodeLibrary;
using System.Text.RegularExpressions;

namespace SMWHubPluginAPI.PackagesTypes;

public class FolderPackage(string packagePath, CodeType type, CodeScope scope) : IPackage
{
    public CodeType Type { get; } = type;
    public CodeScope Scope { get; } = scope;
    public string DirectoryPath { get; } = Path.GetDirectoryName(packagePath)!;
    public string PackagePath { get; } = packagePath;
    public IEnumerable<string> GetFiles()
    {
        string sourcePath = Path.Combine(Scope.SourceDirectoryPath, DirectoryPath);

        return PackageExtensions._extensions.SelectMany(e =>
            Directory.EnumerateFiles(sourcePath, e, SearchOption.AllDirectories));
    }
    public IPackage? GetSubPackageFromInternalFile(string filepath)
    {
        if (!DirectoryExtension.IsSubFolder(DirectoryPath, Path.GetDirectoryName(filepath)!))
            return null;
        string internalFilePath = Path.Combine(Scope.SourceDirectoryPath, filepath);
        if (Directory.Exists(internalFilePath))
            return new FolderPackage(filepath, Type, Scope);
        if (PackageExtensions._rootExtensions.Contains(Path.GetExtension(filepath).ToLower()) &&
            File.Exists(filepath))
            return new FolderPackage(filepath, Type, Scope);
        Match m = CompressedPackage.RootRegex().Match(filepath);
        if (m.Success)
            return new CompressedPackage(m.Value, Type, Scope).GetSubPackageFromInternalFile(filepath);
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
