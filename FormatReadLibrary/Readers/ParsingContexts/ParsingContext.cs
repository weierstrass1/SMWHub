using FormatReadLibrary.LineContexts;
using Validations;

namespace FormatReadLibrary.Readers.ParsingContexts;
public abstract class ParsingContext(LineContext context) : StateValidator
{
    public LineContext LineContext { get; private set; } = context;
    public abstract ValidationResult ProcessEntry();
}
