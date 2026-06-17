using StateMachine.Attributes;
using StateMachine.Interfaces;
using System.Reflection;

namespace StateMachine;

public abstract class Validator
{
    public Validator(IHaveState context)
    {
        validateAttributes(context);
    }
    protected Validator() { }
    protected void validateAttributes(IHaveState context)
    {
        var requirements =
            GetType().GetCustomAttributes<RequiresStateVariableAttribute>();
        foreach (var require in requirements)
        {
            require.Validator.Validate(context);
        }
    }
}
