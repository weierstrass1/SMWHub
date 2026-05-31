using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

[RequiresStateVariable("Entries")]
[RequiresStateVariable("ID", typeof(int?))]
public class ValidateDuplicateID<T> : Validator
{
    private readonly FileEnumeratorWithLog _fileEnumerator;
    public ValidateDuplicateID(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : base(context)
    {
        _fileEnumerator = fileEnumerator;
        if (!context.State.HasVariableOfType<Dictionary<int, T>>("Entries"))
            throw new KeyNotFoundException($"Missing \"Entries\" variable of type {getFriendlyName(typeof(Dictionary<int, T>))} in {getFriendlyName(context.GetType())}'s state.");
    }
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var dictionary = state.Get<Dictionary<int, T>>("Entries")!;
        var id = state.Get<int>("ID")!;
        if (dictionary!.ContainsKey(id))
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Repeated ID"));
            return false;
        }
        return true;
    }
}
