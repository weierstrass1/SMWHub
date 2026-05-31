using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
using System.IO;
namespace FormatReadLibrary.Readers.Validators;
public class ValidateTitleIsNotRepeated(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly (string, Type)[] _varNames = [
                ("CheckedTitle",typeof(Dictionary<string, bool>)),
            ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override  bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var checkedTitle = state.Get<Dictionary<string, bool>>("CheckedTitle")!;
        var lowerline = _fileEnumerator.Current.ToLower().Trim();
        if (checkedTitle[lowerline])
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Repeated List Title"));
            return false;
        }
        return true;
    }
}
