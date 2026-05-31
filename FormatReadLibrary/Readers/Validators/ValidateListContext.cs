using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public class ValidateListContext<T>(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly (string, Type)[] _varNames = [
            ("WasProcessed",typeof(bool)),
        ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var wasProcessed = state.Get<bool>("WasProcessed") as bool?;
        if (wasProcessed == null)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "List doesn't contain a title"));
            return false;
        }

        return true;
    }
}
