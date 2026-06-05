using FormatReadLibrary.Logging.LoggingRegisters;
using FormatReadLibrary.Readers.Enumerators;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateEntryVariables(FileEnumeratorWithLog fileEnumerator, bool allowedVariables = false) : Validator()
{
    private static readonly VariableValidator variableValidator = new("Values", typeof(int[]));
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly bool _allowedVariables = allowedVariables;
    public override bool Validate(IHaveState ctx)
    {
        variableValidator.Validate(ctx);
        State state = ctx.State;
        int[]? values = state.Get<int[]>("Values");
        return Validate(values);
    }
    public bool Validate(int[]? values)
    {
        if (values == null || values.Length == 0)
            return true;
        if (values.Any(v => v < 0 || v > 255))
        {
            _fileEnumerator.AddSyntaxErrorLog("Variable values must be between 0 and 255 [00-FF]");
            return false;
        }
        if (_allowedVariables)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(i, path, line, "This list doesn't allow variable values"));
            return false;
        }

        return true;
    }
}
