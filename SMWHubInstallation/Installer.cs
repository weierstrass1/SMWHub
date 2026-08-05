using FormatReadLibrary.Readers;
using OneOf;
using SMWHubASMCodeLibrary;
using SMWHubInstallation.DTO;
using SMWHubPluginAPI;
using SMWHubPluginAPI.PackagesTypes;
using System.Reflection;
using System.Text;
using Validations;

namespace SMWHubInstallation;

public class Installer(string configPath)
{
    public static readonly string PLUGIN_DIRECTORY = Path.Combine("_Internal", "Plugins");
    private readonly PathContainerDTO _paths = PathContainerDTO.FromJson(File.ReadAllText(configPath));
    public ValidationResult Install()
    {
        ValidationResult validation = new();

        IEnumerable<string> pluginPaths = Directory.GetDirectories(PLUGIN_DIRECTORY)
            .Select(d => 
            {
                string fname = $"{Path.GetFileName(d)}.dll";
                string? dllFile = Directory.GetFiles(d, "*.dll")
                    .FirstOrDefault(f => Path.GetFileName(f) == fname);
                return dllFile ?? "";
            })
            .Where(f => !string.IsNullOrWhiteSpace(f));
        IEnumerable<ISMWHubPlugin> plugins = pluginPaths
            .SelectMany(p =>
            {
                Assembly asm = Assembly.LoadFrom(p);
                return asm.GetTypes()
                    .Where(t => typeof(ISMWHubPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .Select(t => (ISMWHubPlugin?)Activator.CreateInstance(t))
                    .Where(p => p != null)
                    .Select(p => p!);
            });
        InstallationContext context = new(plugins);
        foreach((var resourcePlugin, var plugin) in context.Resources.OrderByDescending(r => r.Item1.GetPackagePriority))
        {
            context.Packages.AddRange(resourcePlugin.GetPackages(plugin.Context, context));
        }
        foreach ((var resourcePlugin, var plugin) in context.Resources.OrderByDescending(r => r.Item1.ProcessPriority))
        {
            validation.Merge(resourcePlugin.Process(plugin.Context, context));
        }
        /*
        CodeContext codeContext = new(_paths.FolderConfigPath, []);

        SharedCodePathProcessor scpp = new(codeContext);

        List<Code> sharedCodes = scpp.FindSharedCodes();


        List<IPackage> packages = [];

        foreach (var nullableOneOf in getNonRoutinePackages(sharedCodes, scpp))
        {
            if (nullableOneOf == null)
                continue;
            if (nullableOneOf.Value.TryPickT0(out ValidationResult val, out IPackage pkg))
            {
                validation.Merge(val);
                continue;
            }
            packages.Add(pkg);
        }
        var routinePkgs = getRoutinePackages(sharedCodes, scpp, packages);
        */
        return validation;
    }
    private IEnumerable<OneOf<ValidationResult, IPackage>?> getNonRoutinePackages(IEnumerable<Code> sharedCodes, SharedCodePathProcessor scpp)
    {
        foreach (var pkg in getSpritePackages(scpp))
            yield return pkg;
    }
    private IEnumerable<IPackage> getRoutinePackages(IEnumerable<Code> sharedCodes, SharedCodePathProcessor scpp, IEnumerable<IPackage> pkgs)
    {
        IEnumerable<Code> codes = pkgs.SelectMany(IPackage.GetCodes);
        var usedRoutines = getUsedRoutines(sharedCodes, scpp, codes);
        Dictionary<Code, HashSet<Code>> allUsed = [];
        foreach (var r in usedRoutines.SelectMany(v => v.Value.Values))
        {
            var set = usedRoutines.Values.SelectMany(r.GetRoutineDefinesFromCollection)
                .Concat(usedRoutines.Values.SelectMany(r.GetMacroCallFromCollection))
                .Select(v => v.Value).Distinct();
            allUsed.TryAdd(r, []);
            foreach (var code in set)
            {
                allUsed.TryAdd(code, []);
                if (!allUsed[code].Contains(r))
                    allUsed[code].Add(r);
            }
        }
        var sortedRoutines = topologicalSort(allUsed);
        return sortedRoutines
            .Select(c => new SingleCodePackage(c.SourcePath, CodeType.Routines, c.Scope));
    }
    private static List<Code> topologicalSort(Dictionary<Code, HashSet<Code>> dependencies)
    {
        var result = new List<Code>();
        var visited = new HashSet<Code>();
        var visiting = new HashSet<Code>();

        foreach (var node in dependencies.Keys)
        {
            Visit(node);
        }

        return result;

        void Visit(Code node)
        {
            if (visited.Contains(node))
                return;

            if (!visiting.Add(node))
                throw new Exception($"Circular dependency detected involving {node}");

            if (dependencies.TryGetValue(node, out var deps))
            {
                foreach (var dependency in deps)
                {
                    Visit(dependency);
                }
            }

            visiting.Remove(node);
            visited.Add(node);

            result.Add(node);
        }
    }
    private Dictionary<ScopeType, Dictionary<string, Code>> getUsedRoutines(IEnumerable<Code> sharedCodes, SharedCodePathProcessor scpp, IEnumerable<Code> codes)
    {
        /*
        var allRoutines = getRoutineSets(sharedCodes, scpp);
        var usedRoutines = getUsedRoutinesStep(allRoutines, codes);

        Dictionary<ScopeType, Dictionary<string, Code>> addingRoutines;

        bool doWhile = true;

        while(doWhile)
        {
            addingRoutines = getUsedRoutinesStep(allRoutines, usedRoutines.Values.SelectMany(d => d.Values));
            doWhile = false;
            foreach ((ScopeType scope, string name, Code code) in addingRoutines.SelectMany(v => v.Value.Select(dv => (v.Key, dv.Key, dv.Value))))
            {
                if (!usedRoutines.TryGetValue(scope, out var res))
                {
                    res = [];
                    usedRoutines.Add(scope, res);
                    doWhile = true;
                }
                if (res.TryAdd(name, code))
                    doWhile = true;
            }
        }

        return usedRoutines;
        */
        return null;
    }
    private Dictionary<IScopeType, Dictionary<string, Code>> getUsedRoutinesStep(Dictionary<ScopeType, Dictionary<string, Code>> routineSets, IEnumerable<Code> codes)
    {
        /*
        Dictionary<IScopeType, Dictionary<string, Code>> result = []; 
        foreach (var code in codes)
        {
            result.TryAdd(code.Scope.Type, []);
            foreach(var r in code.GetRoutineDefinesFromCollection(routineSets[code.Scope.Type]))
            {
                result[code.Scope.Type].TryAdd(r.Key, r.Value);
            }
            if (code.Scope.Type != ScopeType.Sprite && (code.Scope.Parent == null || code.Scope.Parent.Type != ScopeType.Sprite))
                continue;
            foreach (var r in code.GetMacroCallFromCollection(routineSets[code.Scope.Type]))
            {
                result[code.Scope.Type].TryAdd(r.Key, r.Value);
            }
        }
        return result;
        */
        return null;
    }
    private Dictionary<ScopeType, Dictionary<string, Code>> getRoutineSets(IEnumerable<Code> sharedCodes, SharedCodePathProcessor scpp)
    {
        IEnumerable<Code> routines = sharedCodes.Where(c => c.Type == CodeType.Routines);
        return Enum.GetValues<ScopeType>()
            .ToDictionary(
                s => s,
                s =>
                {
                    CodeScope? scope = null;//scpp.GetScope(s);
                    return scope == null ?
                        [] :
                        getRoutineSet(s, scpp, routines);
                });
    }
    private Dictionary<string, Code> getRoutineSet(ScopeType type, SharedCodePathProcessor scpp, IEnumerable<Code> routines)
    {
        /*Dictionary<string, Code> result = [];
        StringBuilder sb = new();
        foreach (var routine in routines.Where(r => scpp.IsScopedBy(r.Scope.Type, type)))
        {
            sb.Clear();
            sb.Append(routine.Scope.Type == type ?
                        "!" :
                        $"!{routine.Scope.Type}_");
            if (!string.IsNullOrWhiteSpace(routine.BreadCrumb))
                sb.Append($"{routine.BreadCrumb}_");
            sb.Append(Path.GetFileNameWithoutExtension(routine.FilePath));
            result.TryAdd(sb.ToString(), routine);
            if (routine.Scope.Type != ScopeType.Sprite && (routine.Scope.Parent == null || routine.Scope.Parent.Type != ScopeType.Sprite))
                continue;
            sb.Clear();
            sb.Append('%');
            if (!string.IsNullOrWhiteSpace(routine.BreadCrumb))
                sb.Append($"{routine.BreadCrumb.Replace("_", "")}");
            sb.Append(Path.GetFileNameWithoutExtension(routine.FilePath));
            sb.Append("()");
            result.TryAdd(sb.ToString(), routine);
        }
        return result;
        */
        return null;
    }
    private IEnumerable<OneOf<ValidationResult, IPackage>?> getSpritePackages(SharedCodePathProcessor scpp)
    {
        /*
        CommonListReader clr = new([
            new NormalSprite(scpp.GetScope(ScopeType.NormalSprite)!.SourceDirectoryPath),
            new ClusterSprite(scpp.GetScope(ScopeType.ClusterSprite)!.SourceDirectoryPath),
            new ExtendedSprite(scpp.GetScope(ScopeType.ExtendedSprite)!.SourceDirectoryPath)
            ]);
        return clr
            .Read(_paths.SpriteListPath)
            .Select(entry => entry.Value is ValidationResult val ?
                val :
                PackageFactory.CreateInstance(entry.AsT1.Item2.Paths[0].Path,
                    CodeType.Code,
                    scpp.GetScope(ScopeTypeExtension.GetFromDescription(entry.AsT1.Item1)!.Value)!));
        */
        return null;
    }
}
