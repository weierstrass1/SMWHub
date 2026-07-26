using SMWHubInstallation.PackagesTypes;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMWHubInstallation;

public class PackageHashes : IDictionary<string, PackageHash>
{
    [JsonInclude]
    [JsonRequired]
    [JsonPropertyName("Package Hashes")]
    private Dictionary<string, PackageHash> _packageHashes = [];
    public ICollection<string> Keys => _packageHashes.Keys;
    public ICollection<PackageHash> Values => _packageHashes.Values;
    public int Count => _packageHashes.Count;
    public bool IsReadOnly => false;
    public PackageHash this[string key] 
    { 
        get => _packageHashes[key]; 
        set => _packageHashes[key] = value; 
    }
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    public static PackageHashes FromJson(string json)
    {
        return JsonSerializer.Deserialize<PackageHashes>(json)!;
    }
    public void Save(string jsonPath)
    {
        string content = JsonSerializer.Serialize(this, _jsonSerializerOptions);
        File.WriteAllText(jsonPath, content);
    }
    public bool? WasModified(IPackage package)
    {
        if (!_packageHashes.TryGetValue(package.PackagePath, out PackageHash? hash))
            return null;
        return hash.WasModified(package);
    }
    public void Add(PackageHash packageHash)
    {
        _packageHashes.Add(packageHash.PackagePath, packageHash);
    }
    public void Clear()
    {
        _packageHashes.Clear();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _packageHashes.GetEnumerator();
    }
    public void Add(string key, PackageHash value)
    {
        _packageHashes.Add(key, value);
    }
    public bool ContainsKey(string key)
    {
        return _packageHashes.ContainsKey(key);
    }
    public bool Remove(string key)
    {
        return _packageHashes.Remove(key);
    }
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out PackageHash value)
    {
        return _packageHashes.TryGetValue(key, out value);
    }
    public void Add(KeyValuePair<string, PackageHash> item)
    {
        ((ICollection<KeyValuePair<string, PackageHash>>)_packageHashes).Add(item);
    }
    public bool Contains(KeyValuePair<string, PackageHash> item)
    {
        return _packageHashes.Contains(item);
    }
    public void CopyTo(KeyValuePair<string, PackageHash>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, PackageHash>>)_packageHashes).CopyTo(array, arrayIndex);
    }
    public bool Remove(KeyValuePair<string, PackageHash> item)
    {
        return ((ICollection<KeyValuePair<string, PackageHash>>)_packageHashes).Remove(item);
    }
    public IEnumerator<KeyValuePair<string, PackageHash>> GetEnumerator()
    {
        return _packageHashes.GetEnumerator();
    }
}
