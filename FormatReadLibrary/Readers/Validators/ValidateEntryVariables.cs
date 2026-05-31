using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateEntryVariables : Validator
    {
        private readonly (string, Type)[] _varNames = [
                ("Values",typeof(int[])),
                ("AllowVariables", typeof(bool)),
                ("Log",typeof(LogRegisterSystem)),
                ("Path", typeof(string)),
                ("LineIndex", typeof(int)),
                ("FileContentLines", typeof(string[])),
            ];
        protected override (string, Type)[] _variableNames { get => _varNames; }
        public ValidateEntryVariables(ParsingContext ctx) : base(ctx)
        { }
        public override bool Validate(ParsingContext ctx)
        {
            State state = ctx.State;
            int[]? values = state.Get<int[]>("Values");
            if (values == null || values.Length == 0)
                return true;
            if (values.Any(v => v < 0 || v > 255))
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                var path = state.Get<string>("Path")!;
                var i = state.Get<int>("LineIndex")!;
                var fileContentLines = state.Get<string[]>("FileContentLines")!;
                log.Add(new SyntaxError(path, i, fileContentLines[i], "Variable values must be between 0 and 255 [00-FF]"));
                return false;
            }
            bool allowVariables = state.Get<bool>("AllowVariables")!;
            if (!allowVariables)
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                var path = state.Get<string>("Path")!;
                var i = state.Get<int>("LineIndex")!;
                var fileContentLines = state.Get<string[]>("FileContentLines")!;
                log.Add(new SyntaxError(path, i, fileContentLines[i], "This list doesn't allow variable values"));
                return false;
            }

            return true;
        }
    }
}
