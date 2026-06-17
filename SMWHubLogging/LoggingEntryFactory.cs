using LogRegister;
using LogRegister.Interfaces;
using SMWHubLogging.LoggingRegisters;
using SMWHubValidations.FormatValidations;
using System.Collections.ObjectModel;
using Validations;

namespace SMWHubLogging;

public class LoggingEntryFactory
{
    private static readonly ReadOnlyDictionary<string, Func<ValidationError, ILoggingEntry>> _dictionary = new Dictionary<string, Func<ValidationError, ILoggingEntry>>()
    {
        { FormatErrorsMessageTypeKeys.DRAW_INFO_INCONSISTENT_TABLE_SIZES, v => new DrawInfoInconsistentTableSizes(v.Parameters["context"]) },
        { FormatErrorsMessageTypeKeys.DYNAMIC_INFO_SIZE_MISMATCH, v => new DynamicInfoSizeMismatch(v.Parameters["context"], long.Parse(v.Parameters["size1"]), long.Parse(v.Parameters["size2"])) },
        { FormatErrorsMessageTypeKeys.DYNAMIC_INFO_WITHOUT_CHUNKS, v => new DynamicInfoWithoutChunks(v.Parameters["context"]) },
        { FormatErrorsMessageTypeKeys.FAILED_TO_PROCESS_FILE, v => new FailedToProcessFile(v.Context.FilePath) },
        { FormatErrorsMessageTypeKeys.FILE_IS_TOO_BIG, v => new FileIsToBig(v.Context.FilePath, long.Parse(v.Parameters["size"]), long.Parse(v.Parameters["maxSize"])) },
        { FormatErrorsMessageTypeKeys.NOT_ENOUGH_SPACE_IN_ROM, v => new NotEnoughSpaceInROM() },
        { FormatErrorsMessageTypeKeys.RESOURCE_NOT_FOUND, v => new ResourceNotFound(v.Context.FilePath)},
        { FormatErrorsMessageTypeKeys.VALUE_EXCEEDS_LIMIT, v=> new ValueExceedsLimit(v.Parameters["context"], v.Parameters["parameter"], int.Parse(v.Parameters["value"]),  int.Parse(v.Parameters["limit"])) }
    }.AsReadOnly();
    public static ILoggingEntry CreateEntry(ValidationError error)
    {
        if (_dictionary.TryGetValue(error.MessageTypeKey, out Func<ValidationError, ILoggingEntry>? value))
            return value(error);

        ValidationContext context = error.Context;
        return new SyntaxError(context.Line, context.FilePath, context.LineContent,
            new LogEntry(error.MessageTypeKey, error.Parameters.ToDictionary()));
    }
}
