using SMWHubValidations;
using StateMachine;
using StateMachine.Interfaces;
using Validations;

namespace FormatReadLibrary.Readers.StateVariables;

public partial class IntegerIDStateVariable<TValue> : StateValidator, IStateVariable<int>
{
    public int Value { get => State.Get<int>("ID"); set => State.Set("ID", value); }
    public bool CleanOnReset { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (int)value!; }
    public IntegerIDStateVariable(Dictionary<int, TValue> dictionary, int maxID = 255, bool allowMultiID = false)
    {
        State.AddVariable("ID", new StateVariable<int>());
        addValidator(new ValidateEntryID(this, maxID));
        addValidator(new ValidateDuplicateID<int, TValue>(this, dictionary, allowMultiID));
    }
    public ValidationResult Validate() => validate();
}
