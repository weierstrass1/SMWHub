using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public sealed class ValidateDuplicateID<TKey, TValue> : Validator
{
    private readonly FileEnumeratorWithLog _fileEnumerator;
    public ValidateDuplicateID(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : base(context)
    {
        _fileEnumerator = fileEnumerator;
        if (!context.State.HasVariable("ID"))
            throw new KeyNotFoundException($"Missing \"ID\" variable of type {getFriendlyName(typeof(TKey))} in {getFriendlyName(context.GetType())}'s state.");
        if (!context.State.HasVariableOfType<Dictionary<TKey, TValue>>("Entries"))
            throw new KeyNotFoundException($"Missing \"Entries\" variable of type {getFriendlyName(typeof(Dictionary<TKey, TValue>))} in {getFriendlyName(context.GetType())}'s state.");
    }
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var dictionary = state.Get<Dictionary<TKey, TValue>>("Entries")!;
        var id = state.Get<TKey>("ID")!;
        if (dictionary!.ContainsKey(id))
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Repeated ID"));
            return false;
        }
        return true;
    }
}
