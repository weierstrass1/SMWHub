using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using SMWHubSprites.Formats;
using SMWHubSprites.ScopeTypes;
using System.Reflection;
namespace SMWHubSprites;
public class SpritePlugin : ISMWHubPlugin
{
    public PluginContext Context { get; }
    private readonly IEnumerable<IScopeType> _scopes;
    private readonly SpriteResourcePlugin _spriteResourcePlugin;
    private readonly CFGFormatPlugin _cfgFormatPlugin;
    public SpritePlugin()
    {
        var method = typeof(IScopeType)
            .GetMethod(nameof(IScopeType.GetInstance))!;

        _scopes = Assembly.GetAssembly(typeof(SpriteType))!
            .GetTypes()
            .Where(t => typeof(IScopeType).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IScopeType)method.MakeGenericMethod(t).Invoke(null, null)!);
        Context = new(typeof(SpritePlugin).Assembly.GetName().Name!);
        Context.StateData.AddVariable("CFGs", new Dictionary<string, SpriteConfigEntry>());
        _spriteResourcePlugin = new();
        _cfgFormatPlugin = new();
    }
    public IEnumerable<IScopeType> GetScopes()
    {
        foreach(var scope in _scopes)
            yield return scope;
    }
    public IEnumerable<IFormatPlugin> GetFormatPlugins()
    {
        yield return _cfgFormatPlugin;
    }
    public IEnumerable<IResourcePlugin> GetResourcePlugins()
    {
        yield return _spriteResourcePlugin;
    }
}
