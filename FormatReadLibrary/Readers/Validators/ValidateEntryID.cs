using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public class ValidateEntryID(ParsingContext context, FileEnumeratorWithLog fileEnumerator, int maxID = 255) : Validator(context)
{
    private readonly (string, Type)[] _varNames = [
            ("ID",typeof(int)),
        ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly int _maxID = maxID;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var id = state.Get<int>("ID")!;
        if (id > _maxID)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, $"ID is over the maximum value ({_maxID:X2})"));
            return false;
        }

        return true;
    }
}
