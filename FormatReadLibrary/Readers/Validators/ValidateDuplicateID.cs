using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public class ValidateDuplicateID<T>(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly (string, Type)[] _varNames = [
            ("Dictionary", typeof(Dictionary<int, T>)),
            ("ID",typeof(int)),
        ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var dictionary = state.Get<Dictionary<int, T>>("Dictionary")!;
        var id = state.Get<int>("ID")!;
        if (dictionary!.ContainsKey(id))
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Repeated ID"));
            return false;
        }
        return true;
    }
}
