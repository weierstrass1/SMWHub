using System.Text;

namespace SMWHubASMCodeLibrary;

public sealed class CodeScope(string directoryPath, ScopeType type, CodeScope? parent = null)
{
    public readonly CodeScope? Parent = parent;
    public readonly string DirectoryPath = directoryPath;
    public readonly ScopeType Type = type;
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
        
        return $"{scope}: {DirectoryPath}";
    }
}
