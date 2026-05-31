namespace StateMachine;

public class State
{
    private readonly Dictionary<string, IStateVariable> _variables;
    public State()
    {
        _variables = [];
    }
    public void CleanLazyTypes()
    {
        var lazys = _variables
            .Values
            .Where(v =>
                {
                    Type t = v.GetType();
                    return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(LazyStateVariable<>);
                });
        foreach (var lazy in lazys)
        {
            lazy.Value = null;
        }
    }
    public IEnumerable<T> FingByType<T>() where T : IStateVariable
    {
        return _variables
            .Where(v => v.GetType() == typeof(T))
            .Select(v => (T)v.Value);
    }
    public void AddVariable(string key, IStateVariable variable)
    {
        _variables.Add(key, variable);
    }
    public bool HasVariable(string key)
    {
        return _variables.ContainsKey(key);
    }
    public bool? HasVariableOfType<T>(string key)
    {
        if(!HasVariable(key)) 
            return false;
        if (_variables[key] is IStateVariable<T> sv)
            return true;
        if (_variables[key].Value == null)
            return null;
        return _variables[key].Value!.GetType() == typeof(T);
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
