using FormatLibrary;
using SMWHubEnumerators;
using System.Text.RegularExpressions;

namespace SMWHubASMCodeLibrary;

public partial class Code
{
    public readonly string FilePath;
    public readonly string ScopePath;
    public readonly string SourcePath;
    public readonly string BreadCrumb;
    public readonly CodeType Type;
    public readonly CodeScope Scope;
    public IReadOnlySet<string> UsedDefines
    {
        get
        {
            analyzeCode();
            return _usedDefines.AsReadOnly();
        }
    }
    public IReadOnlySet<string> UsedMacros
    {
        get
        {
            analyzeCode();
            return _usedMacros.AsReadOnly();
        }
    }
    public IReadOnlySet<(string type, string path)> IncludedFiles
    {
        get
        {
            analyzeCode();
            return _includedFiles.AsReadOnly();
        }
    }
    public IReadOnlyList<FileSection> EmbeddedFiles
    {
        get
        {
            analyzeCode();
            return _embeddedFiles.AsReadOnly();
        }
    }
    private readonly FileLineReader _reader;
    private readonly HashSet<string> _usedDefines = [];
    private readonly HashSet<string> _usedMacros = [];
    private readonly HashSet<(string type, string path)> _includedFiles = [];
    private readonly List<FileSection> _embeddedFiles = [];
    private bool _analyzed = false;
    public Code(string path, CodeType type, CodeScope scope)
    {
        FilePath = path;
        ScopePath = Path.Combine(scope.ScopeDirectoryPath, path);
        SourcePath = Path.Combine(scope.SourceDirectoryPath, path);
        BreadCrumb = Path.GetDirectoryName(Path.GetRelativePath(
              Path.Combine(scope.SourceDirectoryPath, type == CodeType.Code ?
                  "" :
                  Path.Combine(SharedCodePathProcessor.SHARED_CODE_DIRECTORY, type.ToString())),
                  SourcePath))!
              .Replace('\\', '_').Replace('/', '_');
        Type = type;
        Scope = scope;
        _reader = new(SourcePath);
    }
    public IEnumerable<CodeLine> ReadLines(CodeLine? parent = null)
    {
        int i = 1;
        foreach(var line in _reader)
        {
            yield return new(FormatCleaner.CleanLine(line), this, SourcePath, i, parent);
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
    private void analyzeCode()
    {
        if (_analyzed)
            return;
        string embeddedName = "";
        string embeddedType = "";
        bool inEmbedded = false;
        int startEmbeddedSection = 0;
        foreach (var line in ReadLines())
        {
            if (processEmbeddedEnd(line, embeddedName, embeddedType, startEmbeddedSection, ref inEmbedded))
                continue;
            if (processDefines(line))
                continue;
            if (processMacros(line))
                continue;
            if (processFileInclude(line))
                continue;
            (embeddedName, embeddedType) = processEmbeddedStart(line, ref inEmbedded, ref startEmbeddedSection);
        }
        _analyzed = true;
    }
    public override int GetHashCode()
    {
        return SourcePath.GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        if(obj is Code c)
            return StringComparer.OrdinalIgnoreCase.Equals(SourcePath, c.SourcePath);
        return base.Equals(obj);
    }
    private bool processDefines(CodeLine line)
    {
        return processMultiMatch(line, defineUse(), _usedDefines);
    }
    private bool processMacros(CodeLine line)
    {
        return processMultiMatch(line, macroUse(), _usedMacros);
    }
    private bool processFileInclude(CodeLine line)
    {
        string groupType, groupPath;
        Match? m = _includeRegex?.Match(line.Content);
        if (m == null || !m.Success)
            return false;
        groupType = m.Groups.Keys.First(k => k.StartsWith("type"));
        groupPath = m.Groups.Keys.First(k => k.StartsWith("path"));
        _includedFiles.Add((m.Groups[groupType].Value, m.Groups[groupPath].Value));
        return true;
    }
    private (string embeddedName, string embeddedType) processEmbeddedStart(CodeLine line, ref bool inEmbedded, ref int startEmbeddedSection)
    {
        Match? m = _embeddedRegex?.Match(line.Content);
        if (m == null || !m.Success)
            return ("", "");
        inEmbedded = true;
        string embeddedName = m.Groups["name"].Success ?
            m.Groups["name"].Value :
            Path.GetFileNameWithoutExtension(FilePath);
        string embeddedType = m.Groups["type"].Value;
        startEmbeddedSection = line.LineNumber - 1;
        return (embeddedName, embeddedType);
    }
    private bool processEmbeddedEnd(CodeLine line, string embeddedName, string embeddedType, int startEmbeddedSection, ref bool inEmbedded)
    {
        if (!inEmbedded || !endEmbedded().IsMatch(line.Content))
            return false;

        inEmbedded = false;
        _embeddedFiles.Add(new(embeddedName, FilePath, startEmbeddedSection, line.LineNumber - 1)
        {
            Extension = null,
            Format = embeddedType
        });
        return true;
    }
    public static void GenerateIncludeRegex(IEnumerable<(string? inc, string? ext)> includeDirectiveNames)
    {
        var validIncludes = includeDirectiveNames.Where(v => v.inc != null && v.ext != null);
        if (!validIncludes.Any())
            return;
        List<string> incs = [];
        foreach ( (var inc, int index) in validIncludes.Select((v, i) => (v, i))) 
        {
            incs.Add(@$"(?<type{index}>{inc.inc})\s+(""(?<pathA{index}>[a-zA-Z][a-zA-Z0-9\/\\]*\{inc.ext})""|(?<pathB{index}>[a-zA-Z][a-zA-Z0-9\/\\]*\{inc.ext}))");
        }
        string pattern = $@"^inc({string.Join('|', [.. incs])})";
        _includeRegex = new Regex(pattern);
    }
    public static void GenerateEmbeddedRegex(IEnumerable<string?> embeddedNames)
    {
        var validEmbeddeds = embeddedNames.Where(v => v != null);
        if (!validEmbeddeds.Any())
            return;
        string embN = string.Join('|', [.. validEmbeddeds]);
        string pattern = @$"^\#Embedded\s+(?<type>({embN}))(\s+(?<name>[a-zA-Z][a-zA-Z0-9]*))?$";
        _embeddedRegex = new Regex(pattern);
    }
    private static bool processMultiMatch(CodeLine line, Regex regex, ISet<string> target)
    {
        MatchCollection matches = regex.Matches(line.Content);
        if (matches.Count == 0)
            return false;
        foreach (Match match in matches)
        {
            target.Add(match.Groups["name"].Value);
        }
        return true;
    }
    private static Regex? _includeRegex;
    private static Regex? _embeddedRegex;
    [GeneratedRegex(@"(?<name>(?>\![a-zA-Z][a-zA-Z0-9_]*))(?!\s*=)")]
    private static partial Regex defineUse();
    [GeneratedRegex(@"(?<name>%[a-zA-Z][a-zA-Z0-9_]*\(.*?\))")]
    private static partial Regex macroUse();
    [GeneratedRegex(@"^\#End\s+Embedded$")]
    private static partial Regex endEmbedded();
}
