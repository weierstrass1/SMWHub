using StateMachine;
using Validations;

namespace SMWHubValidations;

public sealed class ValidateDuplicateID<TKey, TValue> : Validator where TKey : notnull
{
    private readonly static VariableValidator _variableValidator = new("ID", typeof(TKey));
    private readonly Dictionary<TKey, TValue> _entries;
    private readonly Func<TKey, string> _format;
    private readonly bool _allowMultiIDs;
    public ValidateDuplicateID(IHaveState ctx, Dictionary<TKey, TValue> entries, bool allowMultiIDs = false) : base()
    {
        _variableValidator.Validate(ctx);
        _entries = entries;
        _allowMultiIDs = allowMultiIDs;
        _format = key => key.ToString()!;
    }
    public ValidateDuplicateID(IHaveState ctx, Dictionary<TKey, TValue> entries, Func<TKey, string> format, bool allowMultiIDs = false) : base()
    {
        _variableValidator.Validate(ctx);
        _entries = entries;
        _allowMultiIDs = allowMultiIDs;
        _format = format;
    }
    public override ValidationResult Validate(IHaveState ctx)
    {
        _variableValidator.Validate(ctx);
        State state = ctx.State;
        var id = state.Get<TKey>("ID")!;
        ValidationResult validationResult = new();
        if (!_allowMultiIDs && _entries.ContainsKey(id))
            validationResult.AddError(ValidatorMessagetypeKeys.REPEATED_ID, new() { { "id", _format(id) } });

        return validationResult;
    }
}
