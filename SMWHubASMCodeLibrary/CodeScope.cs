using System.Text;

namespace SMWHubASMCodeLibrary;

public sealed class CodeScope
{
    public bool IsLeaf { get; private set; }
    public CodeScope? Parent { get; }
    public string SourceDirectoryPath { get; }
    public string ScopeDirectoryPath { get; }
    public IScopeType Type { get; }
    public CodeScope(string sourceDirectoryPath, string scopeDirectoryPath, IScopeType type)
    {
        IsLeaf = true;
        SourceDirectoryPath = sourceDirectoryPath;
        ScopeDirectoryPath = scopeDirectoryPath;
        Type = type;
        Parent = null;
    }
    public CodeScope(string sourceDirectoryPath, IScopeType type, CodeScope? parent = null)
    {
        IsLeaf = true;
        Parent = parent;
        Type = type;
        string path = "";
        foreach (var current in GoToRoot().Where(g => g.Type.GetType() != typeof(GlobalScopeType)))
        {
            path = Path.Combine(current.Type.Name, path);
            if (current != null)
                current.IsLeaf = false;
        }
        ScopeDirectoryPath = path;
        SourceDirectoryPath = Path.Combine(sourceDirectoryPath, ScopeDirectoryPath);
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
