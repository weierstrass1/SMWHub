using FormatReadLibrary.Readers.Enumerators;
using FormatReadLibrary.Readers.Validators;

namespace FormatReadLibrary.Readers.ParsingContexts;

public abstract class ParsingContext : StateValidator
{
    public FileEnumeratorWithLog FileEnumerator { get; private set; }
    public ParsingContext(FileEnumeratorWithLog fileEnumerator)
    {
        FileEnumerator = fileEnumerator;
    }
    public abstract bool ProcessEntry();
}
