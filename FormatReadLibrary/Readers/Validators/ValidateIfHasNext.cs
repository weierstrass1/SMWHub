using FormatReadLibrary.Logging.LoggingRegisters;
using StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidateIfHasNext(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
    {
        private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
        public override bool Validate(ParsingContext ctx)
        {
            if (!_fileEnumerator.MoveNext())
            {
                _fileEnumerator.AddSyntaxErrorLog("Expected more entries in the file, but reached the end.");
                return false;
            }
            return true;
        }
    }
}
