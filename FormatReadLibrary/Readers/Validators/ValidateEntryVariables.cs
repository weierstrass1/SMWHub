using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("Values", typeof(int[]))]
public class ValidateEntryVariables(ParsingContext context, FileEnumeratorWithLog fileEnumerator, bool allowedVariables = false) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly bool _allowedVariables = allowedVariables;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        int[]? values = state.Get<int[]>("Values");
        if (values == null || values.Length == 0)
            return true;
        if (values.Any(v => v < 0 || v > 255))
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Variable values must be between 0 and 255 [00-FF]"));
            return false;
        }
        if (_allowedVariables)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "This list doesn't allow variable values"));
            return false;
        }

        return true;
    }
}
