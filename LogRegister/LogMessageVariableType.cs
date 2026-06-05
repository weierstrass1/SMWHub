namespace LogRegister;

public abstract class LogMessageVariableType(string name)
{
    public string Name { get; private set; } = name;
    public abstract Type VariableType { get; }
    public abstract void Validate(object value);
}
public sealed class LogMessageVariableType<T>(string name) : LogMessageVariableType(name)
{
    public override Type VariableType => typeof(T);
    public override void Validate(object value)
    {
        if (value is not T)
            throw new ArgumentException($"Incorrect Type: {Name} is {value.GetType().Name}, Expected {typeof(T)}");
    }
}
