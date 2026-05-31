using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
using System.IO;
namespace FormatReadLibrary.Readers.Validators;
public class ValidateTitleIsNotRepeated(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly (string, Type)[] _varNames = [
                ("WasProcessed",typeof(bool)),
            ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override  bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var wasProcessed = state.Get<bool>("WasProcessed") as bool?;
        if (wasProcessed == null || wasProcessed.Value)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Repeated List Title"));
            return false;
        }
        return true;
    }
}
