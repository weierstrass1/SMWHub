using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("Filepath", typeof(string))]
public class ValidateFileExists(ParsingContext context, LogRegisterSystem log) : Validator(context)
{
    private readonly LogRegisterSystem _log = log;
    public override bool Validate(ParsingContext ctx)
    {
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        if (!File.Exists(filepath))
        {
            _log.Add(new ResourceNotFound(filepath));
            return false;
        }
        return true;
    }
}
