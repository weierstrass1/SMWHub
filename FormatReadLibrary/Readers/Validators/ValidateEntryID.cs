using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateEntryID : Validator
    {
        private readonly (string, Type)[] _varNames = [
                ("ID",typeof(int)),
                ("MaxID",typeof(int)),
                ("Log",typeof(LogRegisterSystem)),
                ("Path", typeof(string)),
                ("LineIndex", typeof(int)),
                ("FileContentLines", typeof(string[])),
            ];
        protected override (string, Type)[] _variableNames { get => _varNames; }
        public ValidateEntryID(ParsingContext ctx) : base(ctx)
        { }
        public override bool Validate(ParsingContext ctx)
        {
            State state = ctx.State;
            var id = state.Get<int>("ID")!;
            var maxID = state.Get<int>("MaxID")!;
            if (id > maxID)
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                var path = state.Get<string>("Path")!;
                var i = state.Get<int>("LineIndex")!;
                var fileContentLines = state.Get<string[]>("FileContentLines")!;
                log.Add(new SyntaxError(path, i, fileContentLines[i], $"ID is over the maximum value ({maxID:X2})"));
                return false;
            }

            return true;
        }
    }
}
