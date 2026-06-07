using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers;
using LogRegister;
using SharedCodeLibrary;
using System.Reflection;

namespace SMWHub;
public class Program
{
    private static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, __) => Console.ResetColor();

        string loggingFilePath = Path.Combine("Logging", "LogMessages.json");
        string loggingFileContent = File.ReadAllText(loggingFilePath);
        Assembly categoryAssembly = typeof(Error).Assembly;

        LogRegisterSystem log = new(loggingFileContent, categoryAssembly);

        SharedCode[] files = SharedCodePathProcessor.FindSharedCodes();
        string[] bcs = files.Select(x => x.BreadCrumb).ToArray();
        var macros = SharedMacrosProcessor.GetMacros(files);
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

        RawTextWrapper rawText = new();
        MultiWrapper mw = new();
        mw.Actions += ConsoleWrapper.RenderAction;
        mw.Actions += rawText.RenderAction;
        bool hasErrors = log.HasLogsOfType<Error>();

        LogRenderer renderer = new(log, mw.RenderAction);
        renderer.RenderAll(log.GetEntries(), error: false, verbose: true);

        File.WriteAllText("log.txt", rawText.ToString());
        Console.ResetColor();
        Console.Read();
    }
}