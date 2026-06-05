using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class InsertedPaletteEffect : ILoggingEntry
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => false;
    public string MessageTypeKey => LogMessageTypeKeys.PALETTE_EFFECT_COLLECTION_INSERTED;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public InsertedPaletteEffect(string name, int length)
    {
        Parameters = new Dictionary<string, string>
        {
            { "name", $"'{name}'" },
            { "length", $"{length}" }
        };
    }
}
