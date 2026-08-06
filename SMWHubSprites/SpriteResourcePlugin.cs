using FormatReadLibrary.Readers;
using OneOf.Types;
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
    private readonly List<IScopeType> _scopes;
    public IEnumerable<IScopeType> ScopeTypes
    {
        get
        {
            foreach (var scope in _scopes)
                yield return scope;
        }
    }
    public int GetPackagePriority { get; set; } = 0;
    public int ProcessPriority { get; set; } = 0;
    public int GetPackageDefaultPriority { get; } = 0;
    public int ProcessDefaultPriority { get; } = 0;
    public Func<Code, string>? CustomRoutineDefinition { get; } = (code) => {
        StringBuilder sb = new();
        sb.Append('%');
        if (!string.IsNullOrWhiteSpace(code.BreadCrumb))
            sb.Append($"{code.BreadCrumb.Replace("_", "")}");
        sb.Append(Path.GetFileNameWithoutExtension(code.FilePath));
        sb.Append("()");
        return sb.ToString();
    };
    public SpriteResourcePlugin()
    {
        var method = typeof(IScopeType)
            .GetMethod(nameof(IScopeType.GetInstance))!;

        _scopes = [.. Assembly.GetAssembly(typeof(SpriteType))!
            .GetTypes()
            .Where(t => typeof(IScopeType).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IScopeType)method.MakeGenericMethod(t).Invoke(null, null)!)];
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
}
