using StateMachine;
using Validations.Interfaces;

namespace Validations;

public abstract class VariableValidation : Validator
{
    public VariableValidation(IValidationState context) : base(context)
    {
    }
    protected VariableValidation()
    {
    }
    public abstract ValidationResult Validate(IValidationState ctx);
}
