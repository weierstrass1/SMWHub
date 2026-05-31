using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
namespace FormatReadLibrary.Readers.Validators;
public class ValidateTitleIsNotRepeated : Validator
{
    private readonly (string, Type)[] _varNames = [
                ("CheckedTitle",typeof(Dictionary<string, bool>)),
                ("Log",typeof(LogRegisterSystem)),
                ("Path", typeof(string)),
                ("LineIndex", typeof(int)),
                ("FileContentLines", typeof(string[])),
            ];
    protected override (string, Type)[] _variableNames { get => _varNames; }
    public ValidateTitleIsNotRepeated(ParsingContext ctx) : base(ctx)
    { }
    public override  bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var checkedTitle = state.Get<Dictionary<string, bool>>("CheckedTitle")!;
        var i = state.Get<int>("LineIndex")!;
        var fileContentLines = state.Get<string[]>("FileContentLines")!;
        var lowerline = fileContentLines[i].ToLower().Trim();
        if (checkedTitle[lowerline])
        {
            var log = state.Get<LogRegisterSystem>("Log")!;
            var path = state.Get<string>("Path")!;
            log.Add(new SyntaxError(path, i, fileContentLines[i], "Repeated List Title"));
            return false;
        }
        return true;
    }
}
