using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using Validations;

namespace SMWHubSharedRoutines
{
    public class SharedResource : IResourcePlugin
    {
        public IEnumerable<IScopeType> ScopeTypes { get; } = [];
        public int GetPackagePriority { get; set; } = int.MinValue;
        public int ProcessPriority { get; set; } = int.MaxValue;
        public int GetPackageDefaultPriority { get; } = int.MinValue;
        public int ProcessDefaultPriority {  get; } = int.MaxValue;
        public Func<Code, string>? CustomRoutineDefinition { get; } = null;
        public IEnumerable<IPackage> GetPackages(PluginContext pluginContext, InstallationContext context)
        {
            return [];
        }
        public ValidationResult Process(PluginContext pluginContext, InstallationContext context)
        {
            return new ValidationResult();
        }
    }
}
