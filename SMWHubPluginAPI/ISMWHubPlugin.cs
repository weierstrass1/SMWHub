using SMWHubASMCodeLibrary;
using ZWXStateMachine;

namespace SMWHubPluginAPI;
/// <summary>
/// Represents a plugin that can be used to extend the functionality of SMWHub. It can provide format plugins and resource plugins, and it has a context that can be used to store state data.
/// </summary>
public interface ISMWHubPlugin
{
    /// <summary>
    /// Gets the context of the plugin, which can be used to store shared data between Resource Plugins and Format Plugins.
    /// </summary>
    public PluginContext Context { get; }
    public IPatchPlugin? GetDefaultPatchPlugin();
    /// <summary>
    /// Gets the format plugins provided by the plugin.
    /// </summary>
    /// <returns>A collection of format plugins.</returns>
    public IEnumerable<IFormatPlugin> GetFormatPlugins();
    /// <summary>
    /// Gets the resource plugins provided by the plugin.
    /// </summary>
    /// <returns>A collection of resource plugins.</returns>
    public IEnumerable<IResourcePlugin> GetResourcePlugins();
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    /// <param name="smwplugin">The plugin for which to get the version.</param>
    /// <returns>The version of the plugin.</returns>
    public static string GetVersion(ISMWHubPlugin smwplugin)
    {
        return smwplugin.GetType().Assembly.GetName().Version?.ToString() ?? "???";
    }
    /// <summary>
    /// Gets the scope types provided by the resource plugins of the plugin.
    /// </summary>
    /// <param name="smwplugin">The plugin for which to get the scope types.</param>
    /// <returns>A collection of scope types.</returns>
    public static IEnumerable<IScopeType> GetScopes(ISMWHubPlugin smwplugin)
    {
        return smwplugin.GetResourcePlugins()
            .SelectMany(rp => rp.ScopeTypes);
    }
    /// <summary>
    /// Validates if the plugin context contains the specific data needed by Format Plugins and Resource Plugins.
    /// </summary>
    /// <param name="plugin">The plugin to validate.</param>
    public static void Validate(ISMWHubPlugin plugin)
    {
        foreach (var formatPlugin in plugin.GetFormatPlugins())
        {
            Validator.Validate(formatPlugin.GetType(), plugin.Context);
        }
        foreach (var resourcePlugin in plugin.GetResourcePlugins())
        {
            Validator.Validate(resourcePlugin.GetType(), plugin.Context);
        }
    }
}
