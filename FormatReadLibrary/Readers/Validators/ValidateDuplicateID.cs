using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateDuplicateID<T> : Validator
    {
        private readonly (string, Type)[] _varNames = [
                ("Dictionary", typeof(Dictionary<int, T>)),
                ("ID",typeof(int)),
                ("Log",typeof(LogRegisterSystem)),
                ("Path", typeof(string)),
                ("LineIndex", typeof(int)),
                ("FileContentLines", typeof(string[])),
            ];
        protected override (string, Type)[] _variableNames { get => _varNames; }
        public ValidateDuplicateID(ParsingContext ctx) : base(ctx) 
        { }
        public override bool Validate(ParsingContext ctx)
        {
            State state = ctx.State;
            var dictionary = state.Get<Dictionary<int, T>>("Dictionary")!;
            var id = state.Get<int>("ID")!;
            if (dictionary!.ContainsKey(id))
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                var path = state.Get<string>("Path")!;
                var i = state.Get<int>("LineIndex")!;
                var fileContentLines = state.Get<string[]>("FileContentLines")!;
                log.Add(new SyntaxError(path, i, fileContentLines[i], "Repeated ID"));
                return false;
            }
            return true;
        }
    }
}
