using StateMachine;
using System.Reflection;
namespace FormatReadLibrary.Readers.Validators;

public abstract class Validator
{
    public abstract bool Validate(ParsingContext ctx);
    public Validator(ParsingContext context)
    {
        State state = context.State;
        bool? check;
        MethodInfo method = typeof(State)
            .GetMethod(nameof(State.HasVariableOfType))!;
        MethodInfo genericMethod;
        var requirements =
            GetType().GetCustomAttributes<RequiresStateVariableAttribute>();
        foreach (var require in requirements)
        {
            if(require.ExpectedType == null)
            {
                if (!state.HasVariable(require.VariableName))
                    throw new KeyNotFoundException($"Missing \"{require.VariableName}\" variable in {getFriendlyName(context.GetType())}'s state.");
                continue;
            }
            genericMethod = method!.MakeGenericMethod(require.ExpectedType);
            check = (bool)genericMethod.Invoke(state, [require.VariableName])!;
            if (!check.Value) 
                throw new KeyNotFoundException($"Missing \"{require.VariableName}\" variable of type {getFriendlyName(require.ExpectedType)} in {getFriendlyName(context.GetType())}'s state.");
        }
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
