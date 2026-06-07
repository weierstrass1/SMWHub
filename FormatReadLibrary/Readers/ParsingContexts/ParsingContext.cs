using SMWHubEnumerators;
using Validations;

namespace FormatReadLibrary.Readers.ParsingContexts;

public abstract class ParsingContext : StateValidator
{
    public FileEnumerator FileEnumerator { get; private set; }
    public ParsingContext(FileEnumerator fileEnumerator)
    {
        FileEnumerator = fileEnumerator;
    }
    public abstract bool ProcessEntry();
    protected override ValidationResult getSelfValidatedVariables(string entry)
    {
        return logValidationResult(base.getSelfValidatedVariables(entry));
    }
    protected override ValidationResult validate()
    {
        return logValidationResult(base.validate());
    }
    private ValidationResult logValidationResult(ValidationResult result)
    {
        result.AddLine(FileEnumerator.LineIndex,FileEnumerator.)

        return result;
    }
}
