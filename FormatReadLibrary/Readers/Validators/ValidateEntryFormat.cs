using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateEntryFormat : Validator
    {
        private readonly (string, Type)[] _varNames = [
                ("Match",typeof(Match)),
                ("Log",typeof(LogRegisterSystem)),
                ("Path", typeof(string)),
                ("LineIndex", typeof(int)),
                ("FileContentLines", typeof(string[])),
            ];
        protected override (string, Type)[] _variableNames { get => _varNames; }
        public ValidateEntryFormat(ParsingContext ctx) : base(ctx)
        { }
        public override bool Validate(ParsingContext ctx)
        {
            State state = ctx.State;
            if (!state.Get<Match>("Match")!.Success)
            {
                var log = state.Get<LogRegisterSystem>("Log")!;
                var path = state.Get<string>("Path")!;
                var i = state.Get<int>("LineIndex")!;
                var fileContentLines = state.Get<string[]>("FileContentLines")!;
                log.Add(new SyntaxError(path, i, fileContentLines[i], "Invalid Entry"));
                return false;
            }
            return true;
        }
    }
}
