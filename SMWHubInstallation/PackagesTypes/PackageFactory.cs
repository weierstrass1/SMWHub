using FormatReadLibrary.Readers;
using SMWHubASMCodeLibrary;
using SMWHubInstallation.Packages;
using System.Text.RegularExpressions;
using Validations;

namespace SMWHubInstallation.PackagesTypes;

public static class PackageFactory
{
    public static IPackage? CreateInstance(string packagePath, CodeScope scope, ValidationResult validation)
    {
        if (Directory.Exists(packagePath))
            return new FolderPackage(packagePath, scope);
        MatchCollection matches = CompressedPackage.CompressedFormatRegex().Matches(packagePath);
        if (matches.Count == 1)
            return new CompressedPackage(packagePath, scope);
        if (matches.Count > 1)
        {
            CompressedPackage cpkg = new(CompressedPackage.RootRegex().Match(packagePath).Value, scope);
            return cpkg.GetSubPackageFromInternalFile(packagePath);
        }
        if (Path.GetDirectoryName(packagePath) == null)
            return createInstanceFromSingleFilePath(packagePath, scope, validation);
        try
        {
            Path.GetFullPath(packagePath);
            return new FolderPackage(packagePath, scope);
        }
        catch
        {
            return null;
        }
    }
    private static IPackage? createInstanceFromSingleFilePath(string packagePath, CodeScope scope, ValidationResult validation)
    {
        switch (Path.GetExtension(packagePath))
        {
            case ".asm":
                return new SingleCodePackage(packagePath, scope);
            case ".cfg":
            case ".json":
                validation.Merge(NormalSpriteCFGReader.Read(packagePath, out var config));
                return config != null ?
                    new SpriteCFGPackage(config, scope) :
                    null;
            default:
                return null;
        }
    }
}
