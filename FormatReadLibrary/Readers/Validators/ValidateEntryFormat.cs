using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateEntryFormat : Validator
{
    private readonly FileEnumeratorWithLog _fileEnumerator;

    public ValidateEntryFormat(ParsingContext context, FileEnumeratorWithLog fileEnumerator, string variableName = "Match") : base(context)
    {
        _fileEnumerator = fileEnumerator;
        if (!context.State.HasVariableOfType<Match>(variableName))
            throw new KeyNotFoundException($"Missing \"{variableName}\" variable of type {getFriendlyName(typeof(Match))} in {getFriendlyName(context.GetType())}'s state.");
    }

    public override bool Validate(ParsingContext ctx)
    {
        Match match = ctx.State.Get<Match>("Match")!;
        if (!match.Success)
        {
            _fileEnumerator.AddSyntaxErrorLog("Invalid Entry");
            return false;
        }
        return true;
    }
}
