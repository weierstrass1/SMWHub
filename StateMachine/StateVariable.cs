namespace StateMachine;

public class StateVariable<T> : IStateVariable<T>
{
    public T? Value { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (T?)value; }
}
