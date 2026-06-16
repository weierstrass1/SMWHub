using System.Reflection;
using Validations.Attributes;
using Validations.Interfaces;
namespace Validations;

public abstract class Validator
{
    public abstract ValidationResult Validate(IValidationState ctx);
    public Validator(IValidationState context)
    {
        var requirements =
            GetType().GetCustomAttributes<RequiresStateVariableAttribute>();
        foreach (var require in requirements)
        {
            require.Validator.Validate(context);
        }
    }
    protected Validator() { }
}
