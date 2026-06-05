using FormatReadLibrary.Readers.Enumerators;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public sealed class ValidateDuplicateID<TKey, TValue>(IHaveState context, FileEnumeratorWithLog fileEnumerator, Dictionary<TKey, TValue> entries, bool allowMultiIDs = false) : Validator(context) where TKey : notnull
{
    private readonly static VariableValidator _variableValidator = new("ID", typeof(TKey));
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly Dictionary<TKey, TValue> _entries = entries;
    private readonly bool _allowMultiIDs = allowMultiIDs;
    public override bool Validate(IHaveState ctx)
    {
        _variableValidator.Validate(ctx);
        State state = ctx.State;
        var id = state.Get<TKey>("ID")!;
        return Validate(id);
    }
    public bool Validate(TKey id)
    {
        if (!_allowMultiIDs && _entries.ContainsKey(id))
        {
            _fileEnumerator.AddSyntaxErrorLog("Repeated ID");
            return false;
        }
        return true;
    }
}
