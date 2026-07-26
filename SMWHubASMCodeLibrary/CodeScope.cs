using System.Text;

namespace SMWHubASMCodeLibrary;

public sealed class CodeScope
{
    public CodeScope? Parent { get; }
    public string SourceDirectoryPath { get; }
    public string ScopeDirectoryPath { get; }
    public ScopeType Type { get; }
    public CodeScope(string sourceDirectoryPath, string scopeDirectoryPath, ScopeType type)
    {
        SourceDirectoryPath = sourceDirectoryPath;
        ScopeDirectoryPath = scopeDirectoryPath;
        Type = type;
        Parent = null;
    }
    public CodeScope(string sourceDirectoryPath, ScopeType type, CodeScope? parent = null)
    {
        Parent = parent;
        SourceDirectoryPath = sourceDirectoryPath;
        Type = type;
        string path = "";
        CodeScope? current = this;
        while (current != null && current.Type != ScopeType.Global)
        {
            path = Path.Combine(current.Type.GetDescription(), path);
            current = current.Parent;
        }
        ScopeDirectoryPath = path;
    }
    public IEnumerable<CodeScope> GoToRoot()
    {
        CodeScope? currentScope = this;
        while(currentScope != null)
        {
            yield return currentScope;
            currentScope = currentScope.Parent;
        }
    }
    public override string ToString()
    {
        StringBuilder scope = new();
        CodeScope? curScope = this;
        while (curScope != null) 
        {
            scope.Insert(0, $"/{curScope.Type}");
            curScope = curScope.Parent;
        }
        scope.Remove(0, 1);
        
        return $"{scope}: {SourceDirectoryPath}";
    }
}
