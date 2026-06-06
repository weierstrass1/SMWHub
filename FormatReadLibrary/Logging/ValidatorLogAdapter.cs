using FormatReadLibrary.Logging.Enumerators;
using FormatReadLibrary.Logging.LoggingRegisters;
using FormatReadLibrary.Readers.Validators;
using LogRegister;

namespace FormatReadLibrary.Logging;

public static class ValidatorLogAdapter
{
    public static void LogValidatorResult(FileEnumeratorWithLog fileEnumerator, ValidationResult result)
    {
        foreach(var error in result.Errors)
            fileEnumerator.Log.Add(getEntry(fileEnumerator, error));
    }
    private static ILoggingEntry getEntry(FileEnumeratorWithLog fileEnumerator, ValidationError error)
    {
        return error.MessageTypeKey switch
        {
            LogMessageTypeKeys.RESOURCE_NOT_FOUND => new ResourceNotFound(error["file"]),
            _ => new SyntaxError(fileEnumerator.LineIndex,fileEnumerator.Path,fileEnumerator.Current, 
            new LogEntry(error.MessageTypeKey, error.Parameters.ToDictionary()))
        };
    }
}
