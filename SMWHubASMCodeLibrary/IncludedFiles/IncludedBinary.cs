using SMWHubValidations.StateVariableValidations;
using Validations;

namespace SMWHubASMCodeLibrary.IncludedFiles;

public class IncludedBinary(string filename, int line, Code parent) : IIncludedFile<byte[]>
{
    public int Line { get; private init; } = line;
    public Code Parent { get; private init; } = parent;
    public string IncName => "bin";
    public string Filename { get; private init; } = filename;
    public ValidationResult ConvertIntoFile(out byte[]? file)
    {
        ValidationResult result = new();
        file = File.Exists(Filename) ? File.ReadAllBytes(Filename) : null;

        if(file == null)
        {
            result.Context = new(Parent.SourcePath, Line, $"incbin \"{Filename}\"");
            result.AddError(StateVariableMessageTypeKeys.FILE_NOT_FOUND);
        }
        
        return result;
    }
}
