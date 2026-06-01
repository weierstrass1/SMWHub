namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateIfHasNext(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
    {
        private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
        public override bool Validate(ParsingContext ctx)
        {
            if (!_fileEnumerator.MoveNext())
            {
                _fileEnumerator.AddSyntaxErrorLog("Expected more entries in the file, but reached the end");
                return false;
            }
            return true;
        }
    }
}
