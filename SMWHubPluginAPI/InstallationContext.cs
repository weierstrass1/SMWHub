using SMWHubASMCodeLibrary;
using SMWHubPluginAPI.DTO;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SMWHubPluginAPI;
public class InstallationContext
{
    public static readonly string PLUGINS_CONFIG = Path.Combine("_Internal", "Settings", "Plugins.json");
    public static readonly string SCOPE_DIRECTORIES_CONFIG = Path.Combine("_Internal", "Settings", "FoldersConfig.json");
    public CodeContext CodeContext { get; }
    public List<IPackage> Packages { get; }
    public ReadOnlyCollection<FormatDefinition> FormatDefinitions { get; }
    public ReadOnlyCollection<ISMWHubPlugin> Plugins { get; }
    public ReadOnlyCollection<IScopeType> Scopes { get; }
    public ReadOnlyCollection<(IResourcePlugin resourcePlugin, ISMWHubPlugin plugin)> Resources { get; }
    public ReadOnlyCollection<(IFormatPlugin formatPlugin, ISMWHubPlugin plugin)> Formats { get; }
    public ReadOnlyDictionary<string, (IFormatPlugin formatPlugin, ISMWHubPlugin plugin)> FormatDictionary { get; }
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    public InstallationContext(IEnumerable<ISMWHubPlugin> plugins)
    {
        Plugins = [.. plugins];
        Scopes = [.. plugins.SelectMany(ISMWHubPlugin.GetScopes), GlobalScopeType.GetInstance()];
        Resources = [.. plugins.SelectMany(p => p.GetResourcePlugins().Select(r => (r, p)))];
        Formats = [.. plugins.SelectMany(p => p.GetFormatPlugins().Select(f => (f, p)))];
        FormatDefinitions = [.. Formats.SelectMany(f => f.formatPlugin.FormatDefinitions)];
        Dictionary<string, (IFormatPlugin, ISMWHubPlugin)> formats = [];
        foreach (var format in Formats)
        {
            foreach(var inc in format.formatPlugin.FormatDefinitions)
            {
                if (inc.EmbeddedName != null)
                    formats.TryAdd(inc.EmbeddedName, format);
                if (inc.IncludeDirectiveName != null)
                    formats.TryAdd(inc.IncludeDirectiveName, format);
            }
        }
        FormatDictionary = formats.AsReadOnly();
        CodeContext = new(SCOPE_DIRECTORIES_CONFIG, Scopes);
        Packages = [];
        if(!File.Exists(PLUGINS_CONFIG))
            File.WriteAllText(PLUGINS_CONFIG, "{}");
        Dictionary<string, PluginConfig> resources = JsonSerializer.Deserialize<Dictionary<string, PluginConfig>>(File.ReadAllText(PLUGINS_CONFIG))!;
        string assemblyName;
        foreach(var res in Resources)
        {
            assemblyName = res.GetType().Assembly.GetName().Name!;
            if (!resources.TryGetValue(assemblyName, out PluginConfig? value))
            {
                value = new PluginConfig
                {
                    GetPackagePriority = res.resourcePlugin.GetPackageDefaultPriority,
                    ProcessPriority = res.resourcePlugin.ProcessDefaultPriority
                };
                resources[assemblyName] = value;
            }
            res.resourcePlugin.GetPackagePriority = value.GetPackagePriority;
            res.resourcePlugin.ProcessPriority = value.ProcessPriority;
        }
       string pluginConfig = JsonSerializer.Serialize(resources, _jsonSerializerOptions);
       File.WriteAllText(PLUGINS_CONFIG, pluginConfig);
    }
}
