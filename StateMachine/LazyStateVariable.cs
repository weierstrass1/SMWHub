namespace StateMachine;

public class LazyStateVariable<T>(Func<T> getter) : IStateVariable<T>
{
    public T? Value
    { 
        get
        {
            _value ??= _getter();
            return _value;
        }
        set
        {
            _value = value;
        }
    }
    object? IStateVariable.Value { get => Value; set => Value = (T?)value; }
    private T? _value;
    private readonly Func<T?> _getter = getter;
}
