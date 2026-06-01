using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("ID", typeof(int?))]
public sealed class ValidateEntryID(ParsingContext context, FileEnumeratorWithLog fileEnumerator, int maxID = 255) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly int _maxID = maxID;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var id = state.Get<int>("ID")!;
        if (id > _maxID)
        {
            _fileEnumerator.AddSyntaxErrorLog($"ID is over the maximum value ({_maxID:X2})");
            return false;
        }

        return true;
    }
}
