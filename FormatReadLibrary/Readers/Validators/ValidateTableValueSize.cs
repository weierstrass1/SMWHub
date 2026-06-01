using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatReadLibrary.Readers.Validators
{
    public enum TableValueSize
    {
        db,
        dw,
        dl,
        dd
    }
    internal class ValidateTableValueSize(ParsingContext context, FileEnumeratorWithLog fileEnumerator, TableValueSize valueSize) : Validator(context)
    {
        private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
        private readonly TableValueSize _valueSize = valueSize;
        public override bool Validate(ParsingContext ctx)
        {
            string name = _valueSize.ToString();
            if (_fileEnumerator.Current.Length > 1 && _fileEnumerator.Current[0..2] != name)
            {
                _fileEnumerator.AddLog((i,path,line) => new SyntaxError(path, i, line, $"Should use {name}"));
                return false;
            }
            return true;
        }
    }
}
