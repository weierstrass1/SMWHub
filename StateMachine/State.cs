namespace StateMachine;

public class State
{
    public IReadOnlyDictionary<string, IStateVariable> Variables => _variables.AsReadOnly();
    private readonly Dictionary<string, IStateVariable> _variables;
    public State()
    {
        _variables = [];
    }
    public void Reset()
    {
        foreach (var variable in _variables.Values)
        {
            if (!variable.CleanOnReset)
                continue;
            variable.Value = default;
        }
    }
    public void AddVariable(string key, IStateVariable variable)
    {
        _variables.Add(key, variable);
    }
    public bool HasVariable(string key)
    {
        return _variables.ContainsKey(key);
    }
    public bool HasVariableOfType<T>(string key)
    {
        if(!HasVariable(key)) 
            return false;
        if (_variables[key] is IStateVariable<T>)
            return true;
        if (_variables[key].Value == null)
            return true;
        return _variables[key].Value!.GetType() == typeof(T);
    }
    public T GetVariable<T>(string key) where T : IStateVariable
    {
        return (T)_variables[key];
    }
    public T? Get<T>(string key)
    {
        if (_variables[key] is IStateVariable<T> genVariable)
            return genVariable.Value;
        return (T?)_variables[key].Value;
    }
    public void Set<T>(string key, T? value)
    {
        if (_variables[key] is IStateVariable<T> genVariable)
        {
            genVariable.Value = value;
            return;
        }
        _variables[key].Value = value;
    }
}
