using FormatReadLibrary.Readers.Enumerators;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateEntryFormat : Validator
{
    private readonly FileEnumeratorWithLog _fileEnumerator;
    public ValidateEntryFormat(IHaveState context, FileEnumeratorWithLog fileEnumerator, string variableName = "Match") : base(context)
    {
        _fileEnumerator = fileEnumerator;
        VariableValidator validator = new(variableName, typeof(Match));
        validator.Validate(context);
    }
    public override bool Validate(IHaveState ctx)
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
