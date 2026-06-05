using System.Reflection;
namespace FormatReadLibrary.Readers.Validators;

public abstract class Validator
{
    public abstract bool Validate(IHaveState ctx);
    public Validator(IHaveState context)
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
