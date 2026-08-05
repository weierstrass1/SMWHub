using SMWHubASMCodeLibrary;
using System.Security.Cryptography;

namespace SMWHubPluginAPI;
/// <summary>
/// Represents a package of files that can be processed by a Resource Plugin. 
/// A package can contain multiple files and can be nested within other packages. 
/// The package is associated with a specific CodeType and CodeScope, which define 
/// the type of code it contains and the scope in which it operates.
/// </summary>
public interface IPackage
{
    /// <summary>
    /// Indicates if the package represents a Macros Library, a Defines Library, a Routine or Code.
    /// </summary>
    public CodeType Type { get; }
    /// <summary>
    /// Indicates the scope in which the package operates. The scope defines the context in which the code in the package is valid and can be used.
    /// </summary>
    public CodeScope Scope { get; }
    /// <summary>
    /// Gets the Source Path of the package. This is the path where the package is located in the file system. It can be used to access the files contained in the package.
    /// </summary>
    public string PackagePath { get; }
    /// <summary>
    /// Gets a sub-package from an internal file within the package. If the specified file path corresponds to a sub-package, it returns that sub-package; otherwise, it returns null.
    /// </summary>
    /// <param name="subPackagePath">The path to the internal package.</param>
    /// <returns>If the sub-package exists return that sub package, otherwise return null</returns>
    public IPackage? GetSubPackageFromInternalFile(string subPackagePath);
    /// <summary>
    /// Gets all the files that can be processed by SMWHub.
    /// </summary>
    /// <returns>A list of file paths.</returns>
    public IEnumerable<string> GetFiles();
    /// <summary>
    /// Gets the size of the package in bytes. This can be used to determine the amount of data contained in the package.
    /// </summary>
    /// <returns>A value representing the size of the package in bytes.</returns>
    public long GetSize();
    /// <summary>
    /// Gets a hash code that represents the contents of the package. This can be used to determine if the contents of the package have changed.
    /// </summary>
    /// <returns>The hash code representing the package contents.</returns>
    public string ObtainHashCode();
    /// <summary>
    /// Gets a hash code that represents the contents of a specific file. This can be used to determine if the contents of the file have changed.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>The hash code representing the file contents.</returns>
    public static string GetHashFromFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using SHA512 sha512 = SHA512.Create();

        byte[] hash = sha512.ComputeHash(stream);

        return Convert.ToBase64String(hash);
    }
    /// <summary>
    /// Gets a hash code that represents the contents of multiple files. This can be used to determine if the contents of the files have changed.
    /// </summary>
    /// <param name="paths">The paths to the files.</param>
    /// <returns>The hash code representing the files' contents.</returns>
    public static string GetHashFromMultiplesFiles(IEnumerable<string> paths)
    {
        List<byte> hashes = [];

        foreach (string path in paths.Order())
        {
            using FileStream stream = File.OpenRead(path);
            using SHA512 sha5121 = SHA512.Create();

            hashes.AddRange(sha5121.ComputeHash(stream));
        }
        using SHA512 sha5122 = SHA512.Create();
        byte[] hash = sha5122.ComputeHash([.. hashes]);
        return Convert.ToBase64String(hash);
    }
    /// <summary>
    /// Gets all the code files from the package. This method filters the files in the package to include only those with a ".asm" extension, and creates a Code object for each of them.
    /// </summary>
    /// <param name="pkg">The package to extract code files from.</param>
    /// <returns>A list of Code objects.</returns>
    public static IEnumerable<Code> GetCodes(IPackage pkg)
    {
        return pkg.GetFiles()
            .Where(f => Path.GetExtension(f) == ".asm")
            .Select(f => new Code(Path.GetRelativePath(pkg.Scope.SourceDirectoryPath, f), CodeType.Code, pkg.Scope));
    }
}
