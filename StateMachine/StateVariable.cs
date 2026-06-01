namespace StateMachine;

public class StateVariable<T> : IStateVariable<T>
{
    public bool CleanOnReset { get; set; }
    public T? Value { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (T?)value; }
    public StateVariable(bool cleanOnReset = false)
    {
        Value = default;
        CleanOnReset = cleanOnReset;
    }
    public StateVariable(T? value, bool cleanOnReset = false)
    {
        Value = value;
        CleanOnReset = cleanOnReset;
    }
}
