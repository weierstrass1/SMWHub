using LogRegister;
using Validations;

namespace SMWHubLogging;

public static class ValidatorLogAdapter
{
    public static void LogValidatorResult(LogRegisterSystem log, ValidationResult result)
    {
        foreach(var error in result.Errors)
            log.Add(LoggingEntryFactory.CreateEntry(error));
    }
}
