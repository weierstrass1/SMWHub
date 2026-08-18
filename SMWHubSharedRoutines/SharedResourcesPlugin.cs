using SMWHubPluginAPI;

namespace SMWHubSharedRoutines;

public class SharedResourcesPlugin : ISMWHubPlugin
{
    public PluginContext Context { get; } = new(typeof(SharedResourcesPlugin).Assembly.GetName().Name!);
    private readonly SharedResource _sharedResource;
    public SharedResourcesPlugin()
    {
        _sharedResource = new(Context);
    }
    public IEnumerable<IFormatPlugin> GetFormatPlugins()
    {
        return [];
    }
    public IEnumerable<IResourcePlugin> GetResourcePlugins()
    {
        yield return _sharedResource;
    }
    public IPatchPlugin? GetDefaultPatchPlugin() => null;
}
