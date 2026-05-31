using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.Validators;

public class ValidateEntryFormat(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly (string, Type)[] _varNames = [
            ("Match",typeof(Match)),
        ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        if (!state.Get<Match>("Match")!.Success)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Invalid Entry"));
            return false;
        }
        return true;
    }
}
