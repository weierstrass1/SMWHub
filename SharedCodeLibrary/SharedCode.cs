namespace SharedCodeLibrary;

public class SharedCode(string path, SharedCodeTypes type, SharedCodeScope scope)
{
    public readonly string FilePath = path;
    public readonly SharedCodeTypes Type = type;
    public readonly SharedCodeScope Scope = scope;
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
    public override string ToString()
    {
        return $"{Type}-{Scope.Type}: {FilePath}";
    }
}
