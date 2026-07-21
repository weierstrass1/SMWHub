using FormatLibrary;
using System.Text.RegularExpressions;

namespace SMWHubASMCodeLibrary;

public partial class Code(string path, CodeType type, CodeScope scope)
{
    public readonly string FilePath = path;
    public string FullPath => Path.Combine(scope.DirectoryPath, FilePath);
    public readonly CodeType Type = type;
    public readonly CodeScope Scope = scope;
    public string BreadCrumb
    {
        get
        {
            string breadcrumb = string.Join("", FilePath.Split(Type.ToString())[1..])
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar)[..^4];
            breadcrumb = string.Join('_', [..breadcrumb.Split(Path.DirectorySeparatorChar)
                .Where(v => !string.IsNullOrWhiteSpace(v))]);

            return breadcrumb;
        }
    }
    public IEnumerable<CodeLine> ReadLines(CodeLine? parent = null)
    {
        string fullpath = Path.Combine(Scope.DirectoryPath, FilePath);
        using StreamReader reader = new(fullpath);
        Code aux;
        CodeLine codeline;
        string? line;
        int i = 1;
        Match m;
        string filepath;
        while ((line = reader.ReadLine()) != null)
        {
            line = FormatCleaner.CleanLine(line);
            codeline = new(line, this, fullpath, i, parent);
            m = incsrcRegex().Match(line);
            if (!m.Success)
            {
                yield return codeline;
                i++;
                continue;
            }
            codeline.GenerateCircularIncludeException();
            filepath = m.Groups["filepath1"].Success ? 
                m.Groups["filepath1"].Value : 
                m.Groups["filepath2"].Value;
            aux = new(Path.Combine(Scope.DirectoryPath, filepath), Type, Scope);
            foreach(CodeLine l in aux.ReadLines(codeline))
            {
                yield return l;
            }
            i++;
        }
    }
    public override string ToString()
    {
        return $"{Type}-{Scope.Type}: {FilePath}";
    }

    [GeneratedRegex("^incsrc (\"(?<filepath1>[a-zA-Z0-9-_\\.]+)\"|(?<filepath2>[a-zA-Z0-9-_\\.]+))$")]
    private static partial Regex incsrcRegex();
}
