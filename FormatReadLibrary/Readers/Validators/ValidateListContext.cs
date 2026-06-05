using FormatReadLibrary.Readers.Enumerators;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("SectionWasProcessed", typeof(bool))]
public sealed class ValidateListContext(IHaveState context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(IHaveState ctx)
    {
        State state = ctx.State;
        var wasProcessed = state.Get<bool>("SectionWasProcessed") as bool?;
        if (wasProcessed == null)
        {
            _fileEnumerator.AddSyntaxErrorLog("List doesn't contain a section title");
            return false;
        }

        return true;
    }
}
