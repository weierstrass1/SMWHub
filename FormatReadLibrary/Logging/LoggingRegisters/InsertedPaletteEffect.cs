using FormatReadLibrary.Logging.Categories;
using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class InsertedPaletteEffect : ILoggingRegister
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => false;
    public ILogCategory Category => new Info();
    public string MessageType => "PALETTE EFFECT COLLECTION INSERTED";
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
