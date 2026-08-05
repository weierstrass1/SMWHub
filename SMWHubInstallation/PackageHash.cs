using SMWHubPluginAPI;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SMWHubInstallation;

public class PackageHash
{
    [JsonRequired]
    [JsonPropertyName("ID")]
    public required int ID { get; init; }
    [JsonRequired]
    [JsonPropertyName("Package Path")]
    public required string PackagePath { get; init; }
    [JsonRequired]
    [JsonPropertyName("Size")]
    public required long Size { get; init; }
    [JsonIgnore]
    public string? HashCode
    {
        get
        {
            ObtainHashCode();
            return _hashcode;
        }
    }
    [JsonInclude]
    [JsonRequired]
    [JsonPropertyName("Hash Code")]
    private string? _hashcode;
    [JsonInclude]
    [JsonPropertyName("Obtained Hash")]
    private bool _obtainedHash;
    private readonly IPackage? _package;
    public PackageHash()
    {
    }
    [SetsRequiredMembers]
    public PackageHash(int id, IPackage package)
    {
        ID = id;
        PackagePath = package.PackagePath;
        Size = package.GetSize();
        _package = package;
        _obtainedHash = false;
    }
    public static string GetHashCodeFromFile(IPackage package)
    {        
        return package.ObtainHashCode();
    }
    public void ObtainHashCode()
    {
        if (_obtainedHash || _package == null)
            return;
        _hashcode = GetHashCodeFromFile(_package);
        _obtainedHash = true;
    }
    public bool? WasModified(IPackage package)
    {
        if (package.PackagePath != PackagePath)
            return null;
        return Size != package.GetSize() ||
            HashCode != GetHashCodeFromFile(package);
    }
}
