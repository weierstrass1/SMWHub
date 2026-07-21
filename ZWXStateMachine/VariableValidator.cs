using System.Reflection;
using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

/// <summary>
/// Validates the existence and type of a state variable.
/// </summary>
/// <remarks>
/// Initializes a new variable validator.
/// </remarks>
/// <param name="variableName">
/// The required variable name.
/// </param>
/// <param name="expectedType">
/// The expected variable type, or <see langword="null"/> to skip type validation.
/// </param>
public class VariableValidator(string variableName, Type? expectedType) : IVariableValidator
{
    /// <summary>
    /// Gets the name of the required variable.
    /// </summary>
    public string VariableName { get; } = variableName;
#nullable enable
    /// <summary>
    /// Gets the expected variable type, or <see langword="null"/> if only the
    /// existence of the variable should be validated.
    /// </summary>
    public Type? ExpectedType { get; } = expectedType;
    private readonly static MethodInfo _method = typeof(StateData)
            .GetMethod(nameof(StateData.HasVariableOfType))!;

    /// <summary>
    /// Validates that the required variable exists and matches the expected type.
    /// </summary>
    /// <param name="ctx">
    /// The object whose state will be validated.
    /// </param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the required variable is missing or has an incompatible type.
    /// </exception>
    public void Validate(IHaveStateData ctx)
    {
        if (ExpectedType == null)
        {
            if (!ctx.StateData.HasVariable(VariableName))
                throw new KeyNotFoundException($"Missing \"{VariableName}\" variable in {getFriendlyName(ctx.GetType())}'s state.");
            return;
        }
        MethodInfo genericMethod = _method.MakeGenericMethod(ExpectedType);
        bool check = (bool)genericMethod.Invoke(ctx.StateData, [VariableName])!;
        if (!check)
            throw new KeyNotFoundException($"Missing \"{VariableName}\" variable of type {getFriendlyName(ExpectedType)} in {getFriendlyName(ctx.GetType())}'s state.");

    }
    /// <summary>
    /// Returns a human-readable representation of a type, including generic
    /// type arguments.
    /// </summary>
    protected static string getFriendlyName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        string name = type.Name;
        int index = name.IndexOf('`');

        if (index >= 0)
            name = name[..index];

        string[] args = [.. type
            .GetGenericArguments()
            .Select(getFriendlyName)];

        return $"{name}<{string.Join(", ", args)}>";
    }
}
