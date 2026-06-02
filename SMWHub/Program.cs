using FormatReadLibrary.Infos;
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
            new("Sprites","Sprites"),
            new("Clusters", "Clusters"),
            new("Extendeds", "Extendeds")
            ]);
        reader.Read("slist.txt", log);
        var entries = reader.GetEntries();

        GPSListReader gpsreader = new("blocks");
        gpsreader.Read("list.txt", log);
        var gpsEntries = gpsreader.GetEntries();

        DynamicInfoReader direader = new();
        direader.Read("DKCMasterGnawty.dynamicinfo", log, out DynamicInfo? dynamicInfo);
        direader.Read("SMWVanillaBoo.dynamicinfo", log, out DynamicInfo? dynamicInfo1);


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