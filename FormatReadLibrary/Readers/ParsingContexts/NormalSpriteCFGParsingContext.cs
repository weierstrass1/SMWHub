using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using Validations;

namespace FormatReadLibrary.Readers;

public static partial class NormalSpriteCFGReader
{
    private class NormalSpriteCFGParsingContext : ParsingContext
    {
        private readonly NormalSpriteCFGReaderStateMachine _stateMachine;
        public NormalSpriteCFGParsingContext(FileEnumeratorLineContext context) : base(context)
        {
            StateData.AddVariable("Context", context);
            StateData.AddVariable<ValidationResult>("Validation");
            StateData.AddVariable("Type", 0);
            StateData.AddVariable<byte>("ActLike", 0);
            StateData.AddVariable<byte>("$1656", 0);
            StateData.AddVariable<byte>("$1662", 0);
            StateData.AddVariable<byte>("$166E", 0);
            StateData.AddVariable<byte>("$167A", 0);
            StateData.AddVariable<byte>("$1686", 0);
            StateData.AddVariable<byte>("$190F", 0);
            StateData.AddVariable<byte>("Prop1", 0);
            StateData.AddVariable<byte>("Prop2", 0);
            StateData.AddVariable("FilePath", "");
            StateData.AddVariable("CleanEBAmount", 0);
            StateData.AddVariable("SetEBAmount", 0);
            _stateMachine = new(StateData);
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            StateData.Set("Validation", new ValidationResult()
            {
                Context = Context
            });
            _stateMachine.Execute();
            return StateData.Get<ValidationResult>("Validation")!;
        }
    }
}
