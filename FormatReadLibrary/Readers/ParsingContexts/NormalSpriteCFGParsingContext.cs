using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using StateMachine;
using Validations;

namespace FormatReadLibrary.Readers;

public static partial class NormalSpriteCFGReader
{
    private class NormalSpriteCFGParsingContext : ParsingContext
    {
        private readonly NormalSpriteCFGReaderStateMachine _stateMachine;
        public NormalSpriteCFGParsingContext(FileEnumeratorLineContext context) : base(context)
        {
            State.AddVariable("Context", context);
            State.AddVariable<ValidationResult>("Validation");
            State.AddVariable("Type", 0);
            State.AddVariable<byte>("ActLike", 0);
            State.AddVariable<byte>("$1656", 0);
            State.AddVariable<byte>("$1662", 0);
            State.AddVariable<byte>("$166E", 0);
            State.AddVariable<byte>("$167A", 0);
            State.AddVariable<byte>("$1686", 0);
            State.AddVariable<byte>("$190F", 0);
            State.AddVariable<byte>("Prop1", 0);
            State.AddVariable<byte>("Prop2", 0);
            State.AddVariable("FilePath", "");
            State.AddVariable("CleanEBAmount", 0);
            State.AddVariable("SetEBAmount", 0);
            _stateMachine = new(State);
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            State.Set("Validation", new ValidationResult()
            {
                Context = Context
            });
            _stateMachine.Execute();
            return State.Get<ValidationResult>("Validation")!;
        }
    }
}
