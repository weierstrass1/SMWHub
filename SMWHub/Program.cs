using FormatLibrary.CommonListCategories;
using FormatReadLibrary.Readers;
using LogRegister;
using SMWHubASMCodeLibrary;
using SMWHubInstallation;
using SMWHubLogging;
using SMWHubLogging.Categories;
using SMWHubLogging.Wrappers;
using SMWHubPatchBuilder;
using SMWHubSprites;
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
        
        SharedCodePathProcessor scpp = new(Path.Combine("Settings", "FoldersConfig.json"));
        Code[] files = scpp.FindSharedCodes();

        Code c = new("root.asm", CodeType.ASM, scpp.GetScope(ScopeType.LevelASM));
        SingleCodePatchGenerator.GenerateSingleCode(0, c);

        
        PackageHashes codeHash = [];
        codeHash.Add(new PackageHash(0, c));
        codeHash.Save("hola.json");

        codeHash = PackageHashes.FromJson(File.ReadAllText("hola.json"));
        Console.WriteLine(codeHash.WasModified(c));

        var macros = SharedMacrosProcessor.GetMacros(files);
        CommonListReader reader = new([
            new NormalSprite(Path.Combine("Sprites","Sprites")),
            new ClusterSprite(Path.Combine("Sprites","Clusters")),
            new ExtendedSprite(Path.Combine("Sprites","Extendeds")),
            new OverworldSprite(Path.Combine("OverworldSprites")),
            ]);
        validation.Merge(reader.Read("spritelist.txt"));
        var entries = reader.GetEntries();

        NormalSpriteCFGReader.Read(entries, out var cfgs);

        SpriteProcessor.GenerateExtraByteTable(cfgs);

        SingleCodePatchGenerator.GenerateMacrosAndDefinesIncludes(Path.Combine("..", ".."), files);

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