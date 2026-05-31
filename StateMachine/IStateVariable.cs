namespace StateMachine
{
    public interface IStateVariable
    {
        public object? Value { get; set; }
    }
    public interface IStateVariable<T> : IStateVariable
    {
        public new T? Value { get; set; }
    }
}
