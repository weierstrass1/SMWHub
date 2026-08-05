using LogRegister;
using SMWHubInstallation;
using SMWHubLogging;
using SMWHubLogging.Categories;
using SMWHubLogging.Wrappers;
using System.Reflection;

namespace SMWHub;

public class Program
{
    private static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, __) => Console.ResetColor();

        string loggingFilePath = Path.Combine("_Internal", "Logging", "LogMessages.json");
        string loggingFileContent = File.ReadAllText(loggingFilePath);
        Assembly categoryAssembly = typeof(Error).Assembly;

        LogRegisterSystem log = new(loggingFileContent, categoryAssembly);

        Installer ins = new(Path.Combine("_Internal", "Settings", "PathConfig.json"));

        ValidatorLogAdapter.LogValidatorResult(log, ins.Install());

        bool hasErrors = log.HasLogsOfType<Error>();

        RawTextWrapper rawText = new();
        LogRenderer renderer = new(log, rawText, new ConsoleWrapper());
        renderer.RenderAll(log.GetEntries(), error: false, verbose: true);

        File.WriteAllText("log.txt", rawText.ToString());
        Console.ResetColor();
        Console.Read();
    }
}