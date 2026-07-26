using SMWHubValidations.StateVariableValidations;
using Validations;

namespace SMWHubASMCodeLibrary.IncludedFiles
{
    public class IncludedCode(string filename, int line, Code parent) : IIncludedFile<Code>
    {
        public int Line { get; private init; } = line;
        public Code Parent { get; private init; } = parent;
        public string IncName => "src";
        public string Filename { get; private init; } = filename;
        public ValidationResult ConvertIntoFile(out Code? file)
        {
            ValidationResult result = new();
            file = File.Exists(Filename) ?
                new(Filename, Parent.Type, Parent.Scope) :
                null;

            if (file == null)
            {
                result.Context = new(Parent.SourcePath, Line, $"incsrc \"{Filename}\"");
                result.AddError(StateVariableMessageTypeKeys.FILE_NOT_FOUND);
            }

            return result;
        }
    }
}
