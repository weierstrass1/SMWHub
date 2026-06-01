using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;
namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("SectionWasProcessed", typeof(bool))]
public sealed class ValidateSectionIsNotRepeated(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override  bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var wasProcessed = state.Get<bool>("SectionWasProcessed") as bool?;
        if (wasProcessed == null || wasProcessed.Value)
        {
            _fileEnumerator.AddSyntaxErrorLog("Repeated List Section");
            return false;
        }
        return true;
    }
}
