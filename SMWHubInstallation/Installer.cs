using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using System.Reflection;
using System.Text;
using Validations;

namespace SMWHubInstallation;

public class Installer(string configPath)
{
    public static readonly string PLUGIN_DIRECTORY = Path.Combine("_Internal", "Plugins");
    public ValidationResult Install()
    {
        ValidationResult validation = new();

        InstallationContext context = buildContext();

        processPackages(context);
        processResourcePlugins(validation, context);

        return validation;
    }
    private static void processPackages(InstallationContext context)
    {
        var packagesPerPlugin = getPackages(context);

        PriorityQueue<IPackage, int> packageQueue;
        PriorityQueue<(IResourcePlugin, PriorityQueue<IPackage, int>), int> packagePerPluginCopy =
            new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        packagePerPluginCopy.EnqueueRange(packagesPerPlugin.UnorderedItems);
        IPatchPlugin patchPlugin;
        StringBuilder patchContent;
        string patchOutput;

        while (packagePerPluginCopy.Count > 0)
        {
            (var resourcePlugin, var packageQ) = packagePerPluginCopy.Dequeue();
            packageQueue = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
            packageQueue.EnqueueRange(packageQ.UnorderedItems);
            while (packageQueue.Count > 0)
            {
                var pkg = packageQueue.Dequeue();
                patchPlugin = pkg.PatchPlugin ?? context.DefaultPatchPlugin;
                patchContent = patchPlugin.BuildPatch(context, pkg);
                resourcePlugin.EditInstallationPatch(patchContent, pkg.Scope, context);
                patchOutput = patchPlugin.Insert(context, patchContent);
                resourcePlugin.ProcessInstallationOutput(patchOutput, pkg.Scope, context);
            }
        }
    }
    private static PriorityQueue<(IResourcePlugin, PriorityQueue<IPackage, int>), int> getPackages(InstallationContext context)
    {
        PriorityQueue<(IResourcePlugin, PriorityQueue<IPackage, int>), int> packagesPerPlugin = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        PriorityQueue<IPackage, int> packageQueue;

        IEnumerable<IPackage> packages;
        foreach ((var resourcePlugin, var plugin) in context.Resources.OrderByDescending(r => r.resourcePlugin.GetPackagePriority))
        {
            packageQueue = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
            packagesPerPlugin.Enqueue((resourcePlugin, packageQueue), resourcePlugin.InstallationPriority);
            packages = [.. resourcePlugin.GetPackages(context)];
            packageQueue.EnqueueRange(packages.Select(p => (p, p.Priority.CurrentPriority)));
            context.Packages.AddRange(packages);
        }
        return packagesPerPlugin;
    }
    private static void processResourcePlugins(ValidationResult validation, InstallationContext context)
    {
        foreach ((var resourcePlugin, var plugin) in context.Resources.OrderByDescending(r => r.resourcePlugin.ProcessPriority))
        {
            validation.Merge(resourcePlugin.Process(context));
        }
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
