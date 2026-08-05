using SMWHubASMCodeLibrary;
using Validations;

namespace SMWHubPluginAPI;
/// <summary>
/// Represents a Resource Plugin that can be used to generate packages from a specific scopes and process those packages.
/// </summary>
public interface IResourcePlugin
{
    /// <summary>
    /// Gets the priority of the GetPackage method. This is used to determine the execution order of the GetPackage method when multiple Resource Plugins are used. A higher value indicates a higher priority.
    /// </summary>
    public int GetPackagePriority { get; set; }
    /// <summary>
    /// Gets the priority of the Process method. This is used to determine the execution order of the Process method when multiple Resource Plugins are used. A higher value indicates a higher priority.
    /// </summary>
    public int ProcessPriority { get; set; }
    /// <summary>
    /// Gets the default priority of the GetPackage method.
    /// </summary>
    public int GetPackageDefaultPriority { get; }
    /// <summary>
    /// Gets the default priority of the Process method.
    /// </summary>
    public int ProcessDefaultPriority { get; }
    /// <summary>
    /// Gets the Custom Scope Types used by the plugin. These scopes types are used as a context for the resources installed by the plugin.
    /// </summary>
    public IEnumerable<IScopeType> ScopeTypes { get; }
    /// <summary>
    /// Processes the given packages and returns a ValidationResult indicating whether the processing was successful or not. The ValidationResult contains any errors that occurred during processing.
    /// </summary>
    /// <param name="pluginContext"></param>
    /// <param name="packages"></param>
    /// <returns></returns>
    public ValidationResult Process(PluginContext pluginContext, InstallationContext context);
    public IEnumerable<IPackage> GetPackages(PluginContext pluginContext, InstallationContext context);
}
