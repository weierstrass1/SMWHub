using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers;
using LogRegister;
using SMWHubASMCodeLibrary;
using SMWHubLogging;
using SMWHubLogging.Categories;
using SMWHubLogging.Wrappers;
using System.Reflection;
using Validations;

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

        ValidationResult validation = new();

        SharedCode[] files = SharedCodePathProcessor.FindSharedCodes();
        string[] bcs = [.. files.Select(x => x.BreadCrumb)];
        var macros = SharedMacrosProcessor.GetMacros(files);
        CommonListReader reader = new([
            new("Sprites","Sprites"),
            new("Clusters", "Clusters"),
            new("Extendeds", "Extendeds")
            ]);
        validation.Merge(reader.Read("slist.txt"));
        var entries = reader.GetEntries();

        GPSListReader gpsreader = new("blocks");
        validation.Merge(gpsreader.Read("list.txt"));
        var gpsEntries = gpsreader.GetEntries();

        validation.Merge(DynamicInfoReader.Read("DKCMasterGnawty.dynamicinfo", out DynamicInfoOld? dynamicInfo));
        validation.Merge(DynamicInfoReader.Read("SMWVanillaBoo.dynamicinfo", out DynamicInfoOld? dynamicInfo1));

        ValidatorLogAdapter.LogValidatorResult(log, validation);

        bool hasErrors = log.HasLogsOfType<Error>();

        RawTextWrapper rawText = new();
        LogRenderer renderer = new(log, rawText, new ConsoleWrapper());
        renderer.RenderAll(log.GetEntries(), error: false, verbose: true);

        File.WriteAllText("log.txt", rawText.ToString());
        Console.ResetColor();
        Console.Read();
    }
}