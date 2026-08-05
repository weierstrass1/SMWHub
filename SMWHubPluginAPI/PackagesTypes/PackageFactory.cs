using OneOf;
using SMWHubASMCodeLibrary;
using System.Text.RegularExpressions;
using Validations;

namespace SMWHubPluginAPI.PackagesTypes;

public static class PackageFactory
{
    public static OneOf<ValidationResult, IPackage>? CreateInstance(string packagePath, CodeType type, CodeScope scope)
    {
        packagePath = Path.GetRelativePath(scope.SourceDirectoryPath, packagePath);
        if (!File.Exists(packagePath) && Directory.Exists(packagePath))
            return new FolderPackage(packagePath, type, scope);
        MatchCollection matches = CompressedPackage.CompressedFormatRegex().Matches(packagePath);
        if (matches.Count == 1)
            return new CompressedPackage(packagePath, type, scope);
        if (matches.Count > 1)
        {
            CompressedPackage cpkg = new(CompressedPackage.RootRegex().Match(packagePath).Value, type, scope);
            IPackage? pkg = cpkg.GetSubPackageFromInternalFile(packagePath);
            if (pkg == null)
                return null;
            return OneOf<ValidationResult, IPackage>.FromT1(pkg);
        }
        if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(packagePath)))
            return createInstanceFromSingleFilePath(packagePath, type, scope);
        try
        {
            Path.GetFullPath(packagePath);
            return new FolderPackage(packagePath, type, scope);
        }
        catch
        {
            return null;
        }
    }
    private static OneOf<ValidationResult, IPackage>? createInstanceFromSingleFilePath(string packagePath, CodeType type, CodeScope scope)
    {
        switch (Path.GetExtension(packagePath))
        {
            case ".asm":
                return new SingleCodePackage(packagePath, type, scope);
            default:
                return null;
        }
    }
}
