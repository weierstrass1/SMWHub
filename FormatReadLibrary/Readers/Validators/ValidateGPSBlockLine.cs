using FormatReadLibrary.Readers.Enumerators;

namespace FormatReadLibrary.Readers.Validators;

[RequiresStateVariable("Start", typeof(int))]
[RequiresStateVariable("End", typeof(int))]
public sealed class ValidateGPSBlockLine(IHaveState context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(IHaveState ctx)
    {
        int start = ctx.State.Get<int>("Start");
        int end = ctx.State.Get<int>("End");
        if ($"{end:X2}"[..^1] != $"{start:X2}"[..^1])
        {
            _fileEnumerator.AddSyntaxErrorLog($"Invalid Range ({start:X2}-{end:X2})");
            return false;
        }
        return true;
    }
}
