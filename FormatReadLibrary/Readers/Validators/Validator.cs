using StateMachine;
using System.Reflection;
namespace FormatReadLibrary.Readers.Validators;

public abstract class Validator
{
    protected abstract  (string, Type)[] _variableNames { get; }
    public abstract bool Validate(ParsingContext ctx);
    public Validator(ParsingContext context)
    {
        State state = context.State;
        bool? check;
        MethodInfo method = typeof(State)
            .GetMethod(nameof(State.HasVariableOfType))!;
        MethodInfo genericMethod;
        foreach (var variable in _variableNames)
        {
            genericMethod = method!.MakeGenericMethod(variable.Item2.GetType());
            check = (bool?)genericMethod.Invoke(state, [variable.Item1]);
            if (check != null && check.Value) 
                throw new KeyNotFoundException($"Missing {variable.Item1} of type {getFriendlyName(variable.Item2)}.");
        }
    }
    private static string getFriendlyName(Type type)
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
