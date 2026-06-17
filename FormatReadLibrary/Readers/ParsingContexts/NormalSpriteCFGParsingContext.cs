using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using Validations;

namespace FormatReadLibrary.Readers;

public static partial class NormalSpriteCFGReader
{
    private class NormalSpriteCFGParsingContext : ParsingContext
    {
        public NormalSpriteCFGParsingContext(FileEnumeratorLineContext context) : base(context)
        {
            State.AddVariable("context", context);
            State.AddVariable("Type", 0);
            State.AddVariable<byte>("ActLike", 0);
            State.AddVariable<Tweak1656>("$1656", 0);
            State.AddVariable<Tweak1662>("$1662", 0);
            State.AddVariable<Tweak166E>("$166E", 0);
            State.AddVariable<Tweak167A>("$167A", 0);
            State.AddVariable<Tweak1686>("$1686", 0);
            State.AddVariable<Tweak190F>("$190F", 0);
            State.AddVariable<byte>("Prop1", 0);
            State.AddVariable<byte>("Prop2", 0);
            State.AddVariable("FilePath", "");
            State.AddVariable("CleanEBAmount", 0);
            State.AddVariable("SetEBAmount", 0);

        }
        public override ValidationResult ProcessEntry()
        {
            throw new NotImplementedException();
        }
    }
}
