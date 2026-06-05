using FormatReadLibrary.Readers.Enumerators;

namespace FormatReadLibrary.Readers.Validators;

public class ValidatePathIntegrity(FileEnumeratorWithLog fileEnumerator) : Validator()
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override bool Validate(IHaveState ctx)
    {
        if (_fileEnumerator.Current.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            _fileEnumerator.AddSyntaxErrorLog("Invalid path");
                return false;
        }
        return true;
    }
}
