using FormatReadLibrary.Logging.Categories;
using FormatReadLibrary.Logging.Wrappers;
using FormatReadLibrary.Readers;
using LogRegister;

namespace SMWHub;
public class Program
{
    private static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, __) => Console.ResetColor();

        LogRegisterSystem log = new();

        CommonListReader reader = new([
            ("Sprites","Sprites"),
            ("Clusters", "Clusters"),
            ("Extendeds", "Extendeds")
            ]);
        reader.Read("slist.txt", log);
        var entries = reader.GetEntries();
        LogRenderer renderer = new(Path.Combine("Logging", "LogMessages.json"));
        RawTextWrapper rawText = new();
        MultiWrapper mw = new();
        mw.Actions += ConsoleWrapper.RenderAction;
        mw.Actions += rawText.RenderAction;
        bool hasErrors = log.HasLogsOfType<Error>();
        renderer.RenderAll(log.GetRegisters(), mw.RenderAction, error: false, verbose: true);
        File.WriteAllText("log.txt", rawText.ToString());
        Console.ResetColor();
        Console.Read();
    }
}