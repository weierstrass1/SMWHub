using StateMachine;
using System.Reflection;

namespace Validations;
public record VariableValidator : IVariableValidator
{
    public string VariableName { get; private set; }
    public Type? ExpectedType { get; private set; }
    private readonly static MethodInfo _method = typeof(State)
            .GetMethod(nameof(State.HasVariableOfType))!;
    public VariableValidator(string variableName, Type? expectedType)
    {
        VariableName = variableName;
        ExpectedType = expectedType;
    }
    public void Validate(IHaveState ctx)
    {
        if (ExpectedType == null)
        {
            if (!ctx.State.HasVariable(VariableName))
                throw new KeyNotFoundException($"Missing \"{VariableName}\" variable in {getFriendlyName(ctx.GetType())}'s state.");
            return;
        }
        MethodInfo genericMethod = _method.MakeGenericMethod(ExpectedType);
        bool check = (bool)genericMethod.Invoke(ctx.State, [VariableName])!;
        if (!check)
            throw new KeyNotFoundException($"Missing \"{VariableName}\" variable of type {getFriendlyName(ExpectedType)} in {getFriendlyName(ctx.GetType())}'s state.");

    }
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
