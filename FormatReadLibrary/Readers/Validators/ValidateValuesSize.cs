using FormatReadLibrary.Logging.LoggingRegisters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatReadLibrary.Readers.Validators
{
    [RequiresStateVariable("Values", typeof(int[]))]
    public class ValidateValuesSize(ParsingContext context, FileEnumeratorWithLog fileEnumerator, uint minSize, uint maxSize) : Validator(context)
    {
        private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
        private readonly uint _minSize = minSize;
        private readonly uint _maxSize = maxSize;
        public override bool Validate(ParsingContext ctx)
        {
            var values = ctx.State.Get<int[]>("Values");
            if (values.Length < _minSize || values.Length > _maxSize)
            {
                _fileEnumerator.AddSyntaxErrorLog(_minSize == _maxSize ?
                    $"You Should use {_minSize} values" :
                    $"You Should use between {_minSize} and {_maxSize} values");
                return false;
            }
            return true;
        }
    }
}
