namespace Validations;

public sealed class ValidationContext(string filePath, int line, string lineContent)
{
    public readonly string FilePath = filePath;
    public readonly int Line = line;
    public readonly string LineContent = lineContent;
}
