using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("Match", typeof(Match))]
public class ValidateEntryFormat(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(ParsingContext ctx)
    {
        Match match = ctx.State.Get<Match>("Match")!;
        if (!match.Success)
        {
            _fileEnumerator.AddLog((i, path, line) => new SyntaxError(path, i, line, "Invalid Entry"));
            return false;
        }
        return true;
    }
}
