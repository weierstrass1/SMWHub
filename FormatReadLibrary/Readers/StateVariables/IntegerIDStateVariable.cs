using SMWHubValidations.StateVariableValidations;
using Validations;
using ZWXStateMachine.Interfaces;

namespace FormatReadLibrary.Readers.StateVariables;

public partial class IntegerIDStateVariable<TValue> : StateValidator, IStateVariable<int>
{
    public int Value { get => StateData.Get<int>("ID"); set => StateData.Set("ID", value); }
    public bool CleanOnReset { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (int)value!; }
    public IntegerIDStateVariable(Dictionary<int, TValue> dictionary, int maxID = 255, bool allowMultiID = false)
    {
        StateData.AddVariable<int>("ID");
        addValidator(new ValidateEntryID(this, maxID));
        addValidator(new ValidateDuplicateID<int, TValue>(this, dictionary, allowMultiID));
    }
    public ValidationResult Validate() => validate();
}
