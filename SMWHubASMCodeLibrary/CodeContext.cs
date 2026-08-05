using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMWHubASMCodeLibrary;
public class CodeContext
{
    public IEnumerable<CodeScope> Scopes => _scopes.Values;
    private readonly IReadOnlyDictionary<IScopeType, CodeScope> _scopes;
    private readonly Dictionary<(IScopeType scopedBy, IScopeType type), bool> _scopedBy = [];
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    public CodeContext(string folderConfigPath, IEnumerable<IScopeType> types)
    {
        Dictionary<string, IScopeType> ts = types.ToDictionary(t => t.Name, t => t);

        if(!File.Exists(folderConfigPath))
            File.WriteAllText(folderConfigPath, "{}");

        Dictionary<string, ScopeTypeFolderAndParent> folders = JsonSerializer
            .Deserialize<Dictionary<string, ScopeTypeFolderAndParent>>(
            File.ReadAllText(folderConfigPath))!;

        foreach (var t in types)
        {
            if (folders.ContainsKey(t.Name))
                continue;
            folders.Add(t.Name, new ScopeTypeFolderAndParent()
            {
                Folder = "",
                ParentName = t.DefaultParentName
            });
        }

        Dictionary<IScopeType, CodeScope> res = folders
            .Where(f => f.Value.ParentName == null && ts.ContainsKey(f.Key))
            .ToDictionary(
                f => ts[f.Key], 
                f => new CodeScope(f.Value.Folder, ts[f.Key], null));

        Dictionary<string, ScopeTypeFolderAndParent> remaining = folders
            .Where(f => f.Value.ParentName != null && ts.ContainsKey(f.Key))
            .ToDictionary();
        List<string> remove = [];
        bool doLoop = true;
        IScopeType parType;
        IScopeType myType;
        while(doLoop && remaining.Count != 0)
        {
            doLoop = false;
            foreach(var kvp in remaining)
            {
                parType = ts[kvp.Value.ParentName!];
                if (!res.ContainsKey(parType))
                    continue;
                myType = ts[kvp.Key];
                remove.Add(kvp.Key);
                res.Add(myType, new CodeScope(kvp.Value.Folder, myType, res[parType]));
                doLoop = true;
            }
            foreach (string r in remove)
            {
                remaining.Remove(r);
            }
            remove.Clear();
        }
        _scopes = res.AsReadOnly();

        folders = _scopes.ToDictionary(s => s.Key.Name, s => new ScopeTypeFolderAndParent()
        {
            Folder = s.Value.SourceDirectoryPath,
            ParentName = s.Value.Parent == null ? null : s.Value.Parent.Type.Name
        });
        string cfg = JsonSerializer.Serialize(folders, _jsonSerializerOptions);
        File.WriteAllText(folderConfigPath, cfg);
    }
    public CodeScope? GetScope(IScopeType type)
    {
        _scopes.TryGetValue(type, out CodeScope? scope);
        return scope;
    }
    public bool IsReachableFrom(IScopeType type1, IScopeType type2)
    {
        return IsScopedBy(type1, type2) || IsScopedBy(type2, type1);
    }
    public bool IsScopedBy(IScopeType ScopedBy, IScopeType type)
    {
        if (_scopedBy.TryGetValue((ScopedBy, type), out bool result))
            return result;

        var scope = GetScope(type);
        if (scope == null)
        {
            _scopedBy[(ScopedBy, type)] = ScopedBy == type;
            return ScopedBy == type;
        }
        List<IScopeType> scopedTypes = [];
        foreach (var currentScope in scope.GoToRoot())
        {
            scopedTypes.Add(currentScope.Type);
            if (currentScope.Type != ScopedBy)
                continue;
            foreach (var s in scopedTypes)
                _scopedBy[(s, type)] = true;
            return true;
        }
        _scopedBy[(ScopedBy, type)] = false;
        return false;
    }
}
public class ScopeTypeFolderAndParent
{
    [JsonRequired]
    public required string Folder { get; init; }
    public string? ParentName { get; init; }
}
