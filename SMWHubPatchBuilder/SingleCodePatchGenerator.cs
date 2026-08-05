using SMWHubASMCodeLibrary;
using System.Text;

namespace SMWHubPatchBuilder;

public static partial class SingleCodePatchGenerator
{
    public const int CODE_POINTERS_ROM_ADDRESS = 0x01CD1E;
    public const string INTERNAL_DIRECTORY = "_Internal";
    public static readonly string TEMPLATE_DIRECTORY = Path.Combine(INTERNAL_DIRECTORY, "Templates");
    public static readonly string ASM_DIRECTORY = Path.Combine(INTERNAL_DIRECTORY, "ASM");
    public static readonly string CODE_POINTERS_PATCH_TEMPLATE = Path.Combine(TEMPLATE_DIRECTORY, "CodePointersPatchTemplate.asm");
    public static readonly string SINGLE_CODE_PATCH_TEMPLATE = Path.Combine(TEMPLATE_DIRECTORY, "SingleCodePatchTemplate.asm");
    public static readonly string SINGLE_CODE_PATCH_DIRECTORY = Path.Combine(ASM_DIRECTORY, "Single Code Patches");
    public static readonly string CODE_POINTERS_PATCH_PATH = Path.Combine(SINGLE_CODE_PATCH_DIRECTORY, "CodePointersPatch.asm");
    public static readonly string INCLUDE_DIRECTORY = Path.Combine(ASM_DIRECTORY, "Includes");
    public static readonly string MACROS_INCLUDE_DIRECTORY = Path.Combine(INCLUDE_DIRECTORY, "Macros");
    public static readonly string DEFINES_INCLUDE_DIRECTORY = Path.Combine(INCLUDE_DIRECTORY, "Defines");
    public static void GenerateMacrosAndDefinesIncludes(string relativePath, IEnumerable<Code> macrosAndDefines)
    {
        var macros = macrosAndDefines.Where(c => c.Type == CodeType.Macros);
        var defines = macrosAndDefines.Where(c => c.Type == CodeType.Defines);

        var scopes = macrosAndDefines.Select(c => c.Scope).ToList();
        scopes.AddRange(macrosAndDefines.Select(c => c.Scope));
        scopes = [.. scopes.Distinct()];

        Directory.CreateDirectory(MACROS_INCLUDE_DIRECTORY);
        Directory.CreateDirectory(DEFINES_INCLUDE_DIRECTORY);

        foreach (var scope in scopes)
        {
            File.WriteAllText(Path.Combine(MACROS_INCLUDE_DIRECTORY, $"{scope.Type}.asm"),
                getIncludeFileForScope(Path.Combine(relativePath, ".."), macros, scope));
            File.WriteAllText(Path.Combine(DEFINES_INCLUDE_DIRECTORY, $"{scope.Type}.asm"),
                getIncludeFileForScope(Path.Combine(relativePath, ".."), defines, scope));
        }
    }
    public static void GenerateCodePointersPatch(int codePointers)
    {
        string content = File.ReadAllText(CODE_POINTERS_PATCH_TEMPLATE)
            .Replace("<address>", $"{CODE_POINTERS_ROM_ADDRESS:X6}")
            .Replace("<pointers>", codePointers.ToString());
        Directory.CreateDirectory(SINGLE_CODE_PATCH_DIRECTORY);
        File.WriteAllText(CODE_POINTERS_PATCH_PATH, content);
    }
    private static string getIncludeFileForScope(string relativePath, IEnumerable<Code> codes, CodeScope scope)
    {
        StringBuilder includeFile = new();
        if (scope.Parent != null)
            includeFile.AppendLine($"incsrc \"{scope.Parent.Type}.asm\"");
        var scopeCodes = codes.Where(c => c.Scope.Type == scope.Type);
        if (scopeCodes.Any())
            includeFile.AppendLine();
        foreach (var code in scopeCodes)
        {
            includeFile.AppendLine($"incsrc \"{Path.Combine(relativePath, scope.SourceDirectoryPath, code.FilePath)}\"");
        }
        return includeFile.ToString();
    }
}
