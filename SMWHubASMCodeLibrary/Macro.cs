namespace SMWHubASMCodeLibrary;

public sealed class Macro(string name, Code sharedCode, int line)
{
    public readonly string Name = name;
    public readonly Code SharedCode = sharedCode;
    public readonly int Line = line;
    public override string ToString()
    {
        return $"In {SharedCode.FilePath} at line {Line}: {Name}";
    }
}
