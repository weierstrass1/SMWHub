using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateListContext<T> : Validator
    {
        private readonly (string, Type)[] _varNames = [
                ("Dictionary",typeof(Dictionary<int, T>)),
                ("Log",typeof(LogRegisterSystem)),
                ("Path", typeof(string)),
                ("LineIndex", typeof(int)),
                ("FileContentLines", typeof(string[])),
            ];
        protected override (string, Type)[] _variableNames { get => _varNames; }
        public ValidateListContext(ParsingContext ctx) : base(ctx)
        { }
        public override bool Validate(ParsingContext ctx)
        {
            State state = ctx.State;
            var dictionary = state.Get<Dictionary<int, T>>("Dictionary");
            if (dictionary == null)
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                var path = state.Get<string>("Path")!;
                var i = state.Get<int>("LineIndex")!;
                var fileContentLines = state.Get<string[]>("FileContentLines")!;
                log.Add(new SyntaxError(path, i, fileContentLines[i], "List doesn't contain a title"));
                return false;
            }

            return true;
        }
    }
}
