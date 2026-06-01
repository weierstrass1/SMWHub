using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public sealed class ValidateDuplicateID<TKey, TValue> : Validator where TKey : notnull
{
    private readonly FileEnumeratorWithLog _fileEnumerator;
    private readonly Dictionary<TKey, TValue> _entries;
    public ValidateDuplicateID(ParsingContext context, FileEnumeratorWithLog fileEnumerator, Dictionary<TKey, TValue> entries) : base(context)
    {
        _fileEnumerator = fileEnumerator;
        _entries = entries;
    }
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var id = state.Get<TKey>("ID")!;
        if (_entries.ContainsKey(id))
        {
            _fileEnumerator.AddSyntaxErrorLog("Repeated ID");
            return false;
        }
        return true;
    }
}
