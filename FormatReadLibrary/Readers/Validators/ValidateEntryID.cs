using FormatReadLibrary.Readers.Enumerators;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateEntryID(FileEnumeratorWithLog fileEnumerator, int maxID = 255) : Validator()
{
    private static readonly VariableValidator _variableValidator = new("ID", typeof(int?));
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly int _maxID = maxID;
    public override bool Validate(IHaveState ctx)
    {
        _variableValidator.Validate(ctx);
        State state = ctx.State;
        var id = state.Get<int>("ID")!;
        return Validate(id);
    }
    public bool Validate(int id)
    {
        if (id > _maxID)
        {
            _fileEnumerator.AddSyntaxErrorLog($"ID is over the maximum value ({_maxID:X2})");
            return false;
        }

        return true;
    }
}
