using LogRegister.Interfaces;
using SMWHubValidations.FormatValidations;

namespace SMWHubLogging.LoggingRegisters;

public sealed class ResourceNotFound : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => FormatErrorsMessageTypeKeys.RESOURCE_NOT_FOUND;
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public ResourceNotFound(string resourceName)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{resourceName}'" }
        }.AsReadOnly();
    }
}
