using SharpCompress.Archives;
using SharpCompress.Common;
using SMWHubASMCodeLibrary;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SMWHubPluginAPI.PackagesTypes;

public partial class CompressedPackage(string packagePath, CodeType type, CodeScope scope) : IPackage
{
    public static readonly string DECOMPRESSED_DIRECTORY = Path.Combine("_Internal", "Decompressed");
    public CodeType Type { get; } = type;
    public CodeScope Scope { get; } = scope;
    public string CompressedPackagePath { get; } = RootRegex().Match(packagePath).Value;
    public string PackagePath { get;  } = packagePath;

    private FolderPackage? _folderPackage;
    public IEnumerable<string> GetFiles()
    {
        buildFolderPackage();
        return _folderPackage.GetFiles();
    }
    public IPackage? GetSubPackageFromInternalFile(string filepath)
    {
        buildFolderPackage();
        Match m = CompressedFormatRegex().Match(filepath);
        string path = m.Success ?
            filepath.Remove(m.Index, m.Length) :
            filepath;
        return _folderPackage.GetSubPackageFromInternalFile(path);
    }
    [MemberNotNull(nameof(_folderPackage))]
    private void buildFolderPackage()
    {
        if (_folderPackage != null)
            return;
        using var archive = ArchiveFactory.OpenArchive(Path.Combine(Scope.SourceDirectoryPath,
            CompressedPackagePath));
        string decompressedDir = Scope.ScopeDirectoryPath.StartsWith(DECOMPRESSED_DIRECTORY) ? 
            Scope.ScopeDirectoryPath :
            Path.Combine(DECOMPRESSED_DIRECTORY, Scope.ScopeDirectoryPath);
        string destination = Path.Combine(decompressedDir,
            Path.GetFileNameWithoutExtension(CompressedPackagePath));
        archive.WriteToDirectory(
            destination,
            new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        CodeScope scope = new(decompressedDir, decompressedDir, Scope.Type);
        _folderPackage = new(destination, Type, scope);
    }
    public long GetSize()
    {
        return new FileInfo(CompressedPackagePath).Length;
    }
    public string ObtainHashCode()
    {
        return IPackage.GetHashFromFile(CompressedPackagePath);
    }
    [GeneratedRegex(@"^[\/\\]?([a-zA-Z][a-zA-Z0-9\/\\]*)\.(zipx|zip|rar|7z|gz|tgz|bz2|tbz2|xz|txz|zst|tzst|lz|arj|ar|ace|a|z)([\/\\]|$)")]
    public static partial Regex RootRegex();
    [GeneratedRegex(@"(\.tar)?\.(zipx|zip|rar|7z|gz|tgz|bz2|tbz2|xz|txz|zst|tzst|lz|arj|ar|ace|a|z)(\\|\/|$)")]
    public static partial Regex CompressedFormatRegex();

}
