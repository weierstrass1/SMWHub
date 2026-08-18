using SMWHubASMCodeLibrary;
using SMWHubPluginAPI;
using SMWHubPluginAPI.PackagesTypes;
using System.Text;
using Validations;

namespace SMWHubSharedRoutines
{
    public class SharedResource : IResourcePlugin
    {
        public Priority GetPackagePriority { get; } = int.MinValue;
        public Priority InstallationPriority { get; } = int.MaxValue;
        public Priority ProcessPriority { get; } = int.MaxValue;
        public IEnumerable<IScopeType> ScopeTypes { get; } = [];
        public Func<Code, string>? CustomRoutineDefinition { get; } = null;
        private Dictionary<IScopeType, Dictionary<string, Code>> _allRoutines = [];
        private Dictionary<IScopeType, Dictionary<string, Code>> _usedRoutines = [];
        public bool MustEditScope<T>() where T : IScopeType<T>
        {
            return IScopeType.GetInstance<T>().AllowsSharedResources;
        }
        public IEnumerable<IPackage> GetPackages(PluginContext pluginContext, InstallationContext context)
        {
            var sharedCodes = getSharedCodes(context.CodeContext);
            return getRoutinePackages(sharedCodes, context.CodeContext, context.Packages);
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
        private List<Code> getSharedCodes(CodeContext codeContext)
        {
            List<(CodeScope scope, string directory)> scopeDirsPairs = [.. codeContext
                .Scopes
                //Filters by scopes that allows shared resources and have the Routines directory
                .Where(static s =>
                    s.Type.AllowsSharedResources &&
                    Directory.Exists(Path.Combine(s.SourceDirectoryPath,
                        SharedCodePathProcessor.SHARED_CODE_DIRECTORY,
                        CodeType.Routines.ToString())))
                //Select tuples (Scope, Routines Directories)
                .Select(s => (s, Path.Combine(s.SourceDirectoryPath,
                        SharedCodePathProcessor.SHARED_CODE_DIRECTORY)))];
            List<Code> sharedCodes = [.. scopeDirsPairs
                        .Select(s => (s.scope, Path.Combine(s.directory, CodeType.Routines.ToString())))
                        //Obtain the codes from Routines Directories
                        .SelectMany(d => Directory.EnumerateFiles(d.Item2, "*.asm")
                            .Select(f => new Code(f, CodeType.Routines, d.scope)))];
            sharedCodes.AddRange(scopeDirsPairs
                        .Select(s => (s.scope, Path.Combine(s.directory, CodeType.Macros.ToString())))
                        //Obtain the codes from Macros Directories
                        .SelectMany(d => Directory.EnumerateFiles(d.Item2, "*.asm")
                            .Select(f => new Code(f, CodeType.Macros, d.scope))));
            sharedCodes.AddRange(scopeDirsPairs
                    .Select(s => (s.scope, Path.Combine(s.directory, CodeType.Defines.ToString())))
                    //Obtain the codes from Defines Directories
                    .SelectMany(d => Directory.EnumerateFiles(d.Item2, "*.asm")
                        .Select(f => new Code(f, CodeType.Defines, d.scope))));
            return sharedCodes;
        }
        private IEnumerable<IPackage> getRoutinePackages(IEnumerable<Code> sharedCodes, CodeContext codeContext, IEnumerable<IPackage> pkgs)
        {
            IEnumerable<Code> codes = pkgs.SelectMany(IPackage.GetCodes);
            var usedRoutines = getUsedRoutines(sharedCodes, codeContext, codes);
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
            _usedRoutines = usedRoutines;
            return sortedRoutines
                .Select((c, index) => new SingleCodePackage(c.SourcePath, CodeType.Routines, c.Scope)
                {
                    Priority = -index,
                    OriginPlugin = this
                });
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
        private Dictionary<IScopeType, Dictionary<string, Code>> getUsedRoutines(IEnumerable<Code> sharedCodes, CodeContext codeContext, IEnumerable<Code> codes)
        {
            var allRoutines = getRoutineSets(sharedCodes, codeContext);
            var usedRoutines = getUsedRoutinesStep(allRoutines, codes);

            Dictionary<IScopeType, Dictionary<string, Code>> addingRoutines;

            bool doWhile = true;

            while(doWhile)
            {
                addingRoutines = getUsedRoutinesStep(allRoutines, usedRoutines.Values.SelectMany(d => d.Values));
                doWhile = false;
                foreach ((IScopeType scope, string name, Code code) in addingRoutines.SelectMany(v => v.Value.Select(dv => (v.Key, dv.Key, dv.Value))))
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
        }
        private Dictionary<IScopeType, Dictionary<string, Code>> getUsedRoutinesStep(Dictionary<IScopeType, Dictionary<string, Code>> routineSets, IEnumerable<Code> codes)
        {
            Dictionary<IScopeType, Dictionary<string, Code>> result = []; 
            foreach (var code in codes)
            {
                result.TryAdd(code.Scope.Type, []);
                foreach(var r in code.GetRoutineDefinesFromCollection(routineSets[code.Scope.Type]))
                {
                    result[code.Scope.Type].TryAdd(r.Key, r.Value);
                }
                foreach (var r in code.GetMacroCallFromCollection(routineSets[code.Scope.Type]))
                {
                    result[code.Scope.Type].TryAdd(r.Key, r.Value);
                }
            }
            return result;
        }
        private Dictionary<IScopeType, Dictionary<string, Code>> getRoutineSets(IEnumerable<Code> sharedCodes, CodeContext codeContext)
        {
            IEnumerable<Code> routines = sharedCodes.Where(c => c.Type == CodeType.Routines);
            return codeContext.Scopes
                .Select(s=> s.Type)
                .Where(s => s.AllowsSharedResources)
                .ToDictionary(
                    s => s,
                    s =>
                    {
                        CodeScope? scope = codeContext.GetScope(s);
                        return scope == null ?
                            [] :
                            getRoutineSet(routines, s, codeContext);
                    });
        }
        private Dictionary<string, Code> getRoutineSet(IEnumerable<Code> routines, IScopeType type, CodeContext codeContext)
        {
            Dictionary<string, Code> result = [];
            StringBuilder sb = new();
            foreach (var routine in routines.Where(r => codeContext.IsScopedBy(r.Scope.Type, type)))
            {
                sb.Clear();
                sb.Append(routine.Scope.Type == type ?
                            "!" :
                            $"!{routine.Scope.Type}_");
                if (!string.IsNullOrWhiteSpace(routine.BreadCrumb))
                    sb.Append($"{routine.BreadCrumb}_");
                sb.Append(Path.GetFileNameWithoutExtension(routine.FilePath));
                result.TryAdd(sb.ToString(), routine);

                if (type is ICustomRoutineDefinition crd)
                    result.TryAdd(crd.Define(routine), routine);
            }
            return result;
        }
    }
}
