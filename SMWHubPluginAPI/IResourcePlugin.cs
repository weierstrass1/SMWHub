using SMWHubASMCodeLibrary;
using System.Text;
using Validations;

namespace SMWHubPluginAPI;
/// <summary>
/// Represents a Resource Plugin that can be used to generate packages from a specific scopes and process those packages.
/// </summary>
public interface IResourcePlugin
{
    public bool MustEditScope<T>() where T : IScopeType<T>;
    /// <summary>
    /// Gets the priority of the GetPackage method. This is used to determine the execution order of the GetPackage method when multiple Resource Plugins are used. A higher value indicates a higher priority.
    /// </summary>
    public Priority GetPackagePriority { get; }
    /// <summary>
    /// Gets the priority of the Process method. This is used to determine the execution order of the Process method when multiple Resource Plugins are used. A higher value indicates a higher priority.
    /// </summary>
    public Priority ProcessPriority { get; }
    public Priority InstallationPriority { get; }
    /// <summary>
    /// Gets the Custom Scope Types used by the plugin. These scopes types are used as a context for the resources installed by the plugin.
    /// </summary>
    public IEnumerable<IScopeType> ScopeTypes { get; }
    public void EditInstallationPatch(StringBuilder patch, CodeScope scope, PluginContext pluginContext, InstallationContext context);
    public void ProcessInstallationOutput(string output, CodeScope scope, PluginContext pluginContext, InstallationContext context);
    /// <summary>
    /// Processes the given packages and returns a ValidationResult indicating whether the processing was successful or not. The ValidationResult contains any errors that occurred during processing.
    /// </summary>
    /// <param name="pluginContext"></param>
    /// <param name="packages"></param>
    /// <returns></returns>
    public ValidationResult Process(PluginContext pluginContext, InstallationContext context);
    /// <summary>
    /// Get all the packages related to this plugin.
    /// </summary>
    /// <param name="pluginContext"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public IEnumerable<IPackage> GetPackages(PluginContext pluginContext, InstallationContext context);
}
