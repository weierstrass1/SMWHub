using Validations;

namespace SMWHubASMCodeLibrary.IncludedFiles;

public interface IIncludedFile
{
    public int Line { get; }
    public Code Parent { get; }
    public string IncName { get; }
    public string Filename { get; }
}
public interface IIncludedFile<T> : IIncludedFile
{
    public ValidationResult ConvertIntoFile(out T? file);
}
