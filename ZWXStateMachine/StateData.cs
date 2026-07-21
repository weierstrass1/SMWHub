using System.Collections.ObjectModel;
using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

/// <summary>
/// Represents the shared runtime state of a state machine.
///
/// A <see cref="StateData"/> stores named variables that can be read and
/// modified by <see cref="IStateBahaviour"/> instances while the state machine
/// is running. It acts as a shared data container, allowing different
/// state logics to exchange information without depending on each other
/// directly.
/// </summary>
public class StateData
{
    /// <summary>
    /// Gets a read-only view of the variables stored in this state.
    ///
    /// This collection is primarily intended for inspection. Variables should
    /// be accessed and modified through the <see cref="Get{T}(string)"/> and
    /// <see cref="Set{T}(string, T)"/> methods.
    /// </summary>
    public IReadOnlyDictionary<string, IStateVariable> Variables => new ReadOnlyDictionary<string, IStateVariable>(_variables);
    /// <summary>
    /// Stores the variables associated with this state.
    /// </summary>
    private readonly Dictionary<string, IStateVariable> _variables = [];
    /// <summary>
    /// Adds a new variable of the specified type using its default value.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    /// <param name="key">The unique name of the variable.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if a variable with the same key already exists.
    /// </exception>
    public void AddVariable<T>(string key)
    {
        _variables.Add(key, new StateVariable<T>());
    }
    /// <summary>
    /// Adds a new variable with the specified initial value.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    /// <param name="key">The unique name of the variable.</param>
    /// <param name="value">The initial value of the variable.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if a variable with the same key already exists.
    /// </exception>
    public void AddVariable<T>(string key, T value)
    {
        _variables.Add(key, new StateVariable<T>(value));
    }
    /// <summary>
    /// Adds an existing state variable instance to this state.
    /// </summary>
    /// <param name="key">The unique name of the variable.</param>
    /// <param name="variable">The state variable to add.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if a variable with the same key already exists.
    /// </exception>
    public void AddStateVariable(string key, IStateVariable variable)
    {
        _variables.Add(key, variable);
    }
    /// <summary>
    /// Determines whether a variable with the specified key exists.
    /// </summary>
    /// <param name="key">The name of the variable.</param>
    /// <returns>
    /// <see langword="true"/> if the variable exists; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool HasVariable(string key)
    {
        return _variables.ContainsKey(key);
    }
    /// <summary>
    /// Determines whether a variable with the specified key exists and is
    /// compatible with the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The expected variable type.
    /// </typeparam>
    /// <param name="key">
    /// The name of the variable.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the variable exists and its value is compatible
    /// with <typeparamref name="T"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool HasVariableOfType<T>(string key)
    {
        if (!HasVariable(key))
            return false;
        if (_variables[key] is IStateVariable<T>)
            return true;
        if (_variables[key].Value == null)
            return true;
        return _variables[key].Value!.GetType() == typeof(T);
    }
    /// <summary>
    /// Gets the state variable associated with the specified key.
    /// </summary>
    /// <typeparam name="T">
    /// The expected state variable type.
    /// </typeparam>
    /// <param name="key">
    /// The name of the variable.
    /// </param>
    /// <returns>
    /// The state variable associated with the specified key.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the variable does not exist.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown if the variable cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    public T GetVariable<T>(string key) where T : IStateVariable
    {
        return (T)_variables[key];
    }
    /// <summary>
    /// Gets the value of the specified variable.
    /// </summary>
    /// <typeparam name="T">
    /// The expected value type.
    /// </typeparam>
    /// <param name="key">
    /// The name of the variable.
    /// </param>
    /// <returns>
    /// The value of the variable.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the variable does not exist.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown if the value cannot be converted to <typeparamref name="T"/>.
    /// </exception>
    public T? Get<T>(string key)
    {
        if (_variables[key] is IStateVariable<T> genVariable)
            return genVariable.Value;
        return (T?)_variables[key].Value;
    }
    /// <summary>
    /// Sets the value of the specified variable.
    /// </summary>
    /// <typeparam name="T">
    /// The value type.
    /// </typeparam>
    /// <param name="key">
    /// The name of the variable.
    /// </param>
    /// <param name="value">
    /// The value to assign.
    /// </param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the variable does not exist.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown if the value is incompatible with the variable.
    /// </exception>
    public void Set<T>(string key, T? value)
    {
        if (_variables[key] is IStateVariable<T> genVariable)
        {
            genVariable.Value = value;
            return;
        }
        _variables[key].Value = value;
    }
    /// <summary>
    /// Removes all variables from this state.
    /// </summary>
    public void Clear()
    {
        _variables.Clear();
    }
}
