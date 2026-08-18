using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using System.Reflection;
using Validations;

namespace SMWHubInstallation;

public class Installer(string configPath)
{
    public static readonly string PLUGIN_DIRECTORY = Path.Combine("_Internal", "Plugins");
    public ValidationResult Install()
    {
        ValidationResult validation = new();

        InstallationContext context = buildContext();

        var packagesPerPlugin = getPackages(context);

        PriorityQueue<IPackage, int> packageQueue;
        PriorityQueue<(IResourcePlugin, PriorityQueue<IPackage, int>), int> packagePerPluginCopy =
            new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        packagePerPluginCopy.EnqueueRange(packagesPerPlugin.UnorderedItems);

        while (packagePerPluginCopy.Count > 0)
        {
            (var resourcePlugin, var packageQ) = packagePerPluginCopy.Dequeue();
            packageQueue = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
            packageQueue.EnqueueRange(packageQ.UnorderedItems);
            while (packagePerPluginCopy.Count > 0)
            {
                var pkg = packagePerPluginCopy.Dequeue().Item1;
            }
        }
        foreach ((var resourcePlugin, var plugin) in context.Resources.OrderByDescending(r => r.resourcePlugin.ProcessPriority))
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

    private static PriorityQueue<(IResourcePlugin, PriorityQueue<IPackage, int>), int> getPackages(InstallationContext context)
    {
        PriorityQueue<(IResourcePlugin, PriorityQueue<IPackage, int>), int>  packagesPerPlugin = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        PriorityQueue<IPackage, int> packageQueue;

        IEnumerable <IPackage> packages;
        foreach ((var resourcePlugin, var plugin) in context.Resources.OrderByDescending(r => r.resourcePlugin.GetPackagePriority))
        {
            packageQueue = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
            packagesPerPlugin.Enqueue((resourcePlugin, packageQueue), resourcePlugin.InstallationPriority);
            packages = [.. resourcePlugin.GetPackages(plugin.Context, context)];
            packageQueue.EnqueueRange(packages.Select(p => (p, p.Priority.CurrentPriority)));
            context.Packages.AddRange(packages);
        }
        return packagesPerPlugin;
    }

    private static InstallationContext buildContext()
    {
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
        Code.GenerateIncludeRegex(context.FormatDefinitions.Select(fd => (fd.IncludeDirectiveName, fd.Extension)));
        Code.GenerateEmbeddedRegex(context.FormatDefinitions.Select(fd => fd.EmbeddedName));
        return context;
    }
}
