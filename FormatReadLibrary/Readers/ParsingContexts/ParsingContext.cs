using SMWHubEnumerators;
using Validations;

namespace FormatReadLibrary.Readers.ParsingContexts;

public abstract class ParsingContext(FileEnumerator fileEnumerator) : StateValidator
{
    public FileEnumerator FileEnumerator { get; private set; } = fileEnumerator;
    public abstract ValidationResult ProcessEntry();
}
