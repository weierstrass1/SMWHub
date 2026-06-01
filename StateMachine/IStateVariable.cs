namespace StateMachine;

public interface IStateVariable
{
    public bool CleanOnReset { get; }
    public object? Value { get; set; }
}
public interface IStateVariable<T> : IStateVariable
{
    public new T? Value { get; set; }
}
