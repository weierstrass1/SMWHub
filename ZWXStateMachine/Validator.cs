using System.Reflection;
using ZWXStateMachine.Attributes;
using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

/// <summary>
/// Provides automatic validation based on validation attributes.
///
/// Classes deriving from <see cref="Validator"/> automatically validate
/// their declared requirements during construction.
/// </summary>
public abstract class Validator
{
    /// <summary>
    /// Initializes a new validator and validates the specified context.
    /// </summary>
    /// <param name="context">
    /// The object whose state will be validated.
    /// </param>
    public Validator(IHaveStateData context)
    {
        validateAttributes(context);
    }
    protected Validator() { }
    /// <summary>
    /// Validates all requirements declared through validation attributes.
    /// </summary>
    /// <param name="context">
    /// The object whose state will be validated.
    /// </param>
    protected void validateAttributes(IHaveStateData context)
    {
        var requirements = GetType()
            .GetCustomAttributes<RequiresStateVariableAttribute>();

        foreach (var require in requirements)
        {
            require.Validator.Validate(context);
        }
    }
}
