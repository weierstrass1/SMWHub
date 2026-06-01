using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatReadLibrary.Readers.Validators
{
    public class ValidatePathIntegrity(ParsingContext context, FileEnumeratorWithLog fileEnumerator) : Validator(context)
    {
        private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
        public override bool Validate(ParsingContext ctx)
        {
            if (_fileEnumerator.Current.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                _fileEnumerator.AddSyntaxErrorLog("Invalid path");
                    return false;
            }
            return true;
        }
    }
}
