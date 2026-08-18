using FormatReadLibrary.Readers;
using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using SMWHubSprites.CommonListCategories;
using SMWHubSprites.ScopeTypes;
using System.Reflection;
using System.Text;
using Validations;

namespace SMWHubSprites;

public class SpriteResourcePlugin : IResourcePlugin
{
    public Priority GetPackagePriority { get; } = 0;
    public Priority ProcessPriority { get; } = 0;
    public Priority InstallationPriority { get; } = 0;
    public IEnumerable<IScopeType> ScopeTypes
    {
        get
        {
            foreach (var scope in _scopes)
                yield return scope;
        }
    }
    private readonly List<IScopeType> _scopes;
    public SpriteResourcePlugin()
    {
        var method = typeof(IScopeType)
            .GetMethod(nameof(IScopeType.GetInstance))!;

        _scopes = [.. Assembly.GetAssembly(typeof(SpriteType))!
            .GetTypes()
            .Where(t => typeof(IScopeType).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IScopeType)method.MakeGenericMethod(t).Invoke(null, null)!)];
    }
    public bool MustEditScope<T>() where T : IScopeType<T>
    {
        return ScopeTypes.Any(s => s is T);
    }
    public IEnumerable<IPackage> GetPackages(PluginContext pluginContext, InstallationContext context)
    {
        CodeContext codeContext = context.CodeContext;
        CommonListReader clr = new([
                new NormalSprite(codeContext.GetScope(NormalSpriteType.GetInstance())!.SourceDirectoryPath),
                new ClusterSprite(codeContext.GetScope(ClusterSpriteType.GetInstance())!.SourceDirectoryPath),
                new ExtendedSprite(codeContext.GetScope(ExtendedSpriteType.GetInstance())!.SourceDirectoryPath)
            ]);
        return [];
    }
    public ValidationResult Process(PluginContext pluginContext, InstallationContext context)
    {
        return new ValidationResult();
    }
    public void EditInstallationPatch(StringBuilder patch, CodeScope scope, PluginContext pluginContext, InstallationContext context)
    {
        throw new NotImplementedException();
    }
    public void ProcessInstallationOutput(string output, CodeScope scope, PluginContext pluginContext, InstallationContext context)
    {
        throw new NotImplementedException();
    }
    public static string CustomRoutineDefinition(Code code)
    {
        StringBuilder sb = new();
        sb.Append('%');
        if (!string.IsNullOrWhiteSpace(code.BreadCrumb))
            sb.Append($"{code.BreadCrumb.Replace("_", "")}");
        sb.Append(Path.GetFileNameWithoutExtension(code.FilePath));
        sb.Append("()");
        return sb.ToString();
    }
}
