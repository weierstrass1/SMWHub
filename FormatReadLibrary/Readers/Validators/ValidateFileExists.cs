using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateFileExists : Validator
    {
        private readonly (string, Type)[] _varNames = [
                ("Filepath",typeof(string)),
                ("Log",typeof(LogRegisterSystem))
            ];
        protected override (string, Type)[] _variableNames { get => _varNames; }
        public ValidateFileExists(ParsingContext ctx) : base(ctx)
        { }
        public override bool Validate(ParsingContext ctx)
        {
            State state = ctx.State;
            var filepath = state.Get<string>("Filepath")!;
            if (!File.Exists(filepath))
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                log.Add(new ResourceNotFound(filepath));
                return false;
            }
            return true;
        }
    }
}
