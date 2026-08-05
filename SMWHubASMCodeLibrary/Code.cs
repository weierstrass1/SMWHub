using FormatLibrary;
using System.Text.RegularExpressions;

namespace SMWHubASMCodeLibrary;

public partial class Code(string path, CodeType type, CodeScope scope)
{
    public readonly string FilePath = path;
    public readonly string ScopePath = Path.Combine(scope.ScopeDirectoryPath, path);
    public readonly string SourcePath = Path.Combine(scope.SourceDirectoryPath, path);
    public readonly string BreadCrumb = Path.GetDirectoryName(Path.GetRelativePath(
        Path.Combine(scope.SourceDirectoryPath, type == CodeType.Code ?
            "" :
            Path.Combine(SharedCodePathProcessor.SHARED_CODE_DIRECTORY, type.ToString())),
            Path.Combine(scope.SourceDirectoryPath, path)))!
        .Replace('\\', '_').Replace('/', '_');
    public readonly CodeType Type = type;
    public readonly CodeScope Scope = scope;
    public IReadOnlySet<string> UsedDefines
    {
        get
        {
            analizeCode();
            return _usedDefines.AsReadOnly();
        }
    }
    public IReadOnlySet<string> UsedMacros
    {
        get
        {
            analizeCode();
            return _usedMacros.AsReadOnly();
        }
    }
    private readonly HashSet<string> _usedDefines = [];
    private readonly HashSet<string> _usedMacros = [];
    private bool _analized = false;
    public IEnumerable<CodeLine> ReadLines(CodeLine? parent = null)
    {
        using StreamReader reader = new(SourcePath);

        CodeLine codeline;
        string? line;
        int i = 1;

        while ((line = reader.ReadLine()) != null)
        {
            line = FormatCleaner.CleanLine(line);
            codeline = new(line, this, SourcePath, i, parent);
            yield return codeline;
            i++;
        }
    }
    public Dictionary<string,Code> GetRoutineDefinesFromCollection(Dictionary<string, Code> routines)
    {
        return routines.Where(kvp => UsedDefines.Contains(kvp.Key)).ToDictionary();
    }
    public Dictionary<string, Code> GetMacroCallFromCollection(Dictionary<string, Code> routines)
    {
        return routines.Where(kvp => UsedMacros.Contains(kvp.Key)).ToDictionary();
    }
    public override string ToString()
    {
        return $"{Type}-{Scope.Type}: {FilePath}";
    }
    private void analizeCode()
    {
        if (_analized)
            return;
        string name;
        foreach (var line in ReadLines())
        {
            foreach (Match match in defineUse().Matches(line.Content))
            {
                name = match.Value;
                if (!_usedDefines.Contains(name))
                    _usedDefines.Add(name);
            }
            foreach (Match match in macroUse().Matches(line.Content))
            {
                name = match.Value;
                if (!_usedMacros.Contains(name))
                    _usedMacros.Add(name);
            }
        }
        _analized = true;
    }
    public override int GetHashCode()
    {
        return SourcePath.GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        if(obj is Code c)
            return SourcePath == c.SourcePath;
        return base.Equals(obj);
    }
    [GeneratedRegex(@"(?<name>(?>\![a-zA-Z][a-zA-Z0-9_]*))(?!\s*=)")]
    private static partial Regex defineUse();
    [GeneratedRegex(@"(?<name>\%[a-zA-Z][a-zA-Z0-9_]*\(.*\))")]
    private static partial Regex macroUse();
}
