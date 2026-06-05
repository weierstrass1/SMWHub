using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateFileExists(LogRegisterSystem log) : Validator()
{
    private readonly static VariableValidator _variableValidator = new("Filepath", typeof(string));
    private readonly LogRegisterSystem _log = log;
    public override bool Validate(IHaveState ctx)
    {
        _variableValidator.Validate(ctx);
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        return Validate(filepath);
    }
    public bool Validate(string filepath)
    {
        if (!File.Exists(filepath))
        {
            _log.Add(new ResourceNotFound(filepath));
            return false;
        }
        return true;
    }
}
