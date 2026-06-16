using System.Text.RegularExpressions;
using Validations;

namespace SMWHubASMCodeLibrary;
public static partial class SharedMacrosProcessor
{
    public const string REPEATED_MACRO_NAME = "REPEATED MACRO NAME";
    [GeneratedRegex(@"macro\s+(?<name>[a-zA-Z][a-zA-Z0-9]*)\(\s*([a-zA-Z][a-zA-Z0-9]*(\s*,\s*[a-zA-Z][a-zA-Z0-9]*)*)?\s*\)")]
    private static partial Regex macroSignatureRegex();
    public static Macro[] GetMacros(IEnumerable<SharedCode> sharedCodes)
    {
        string[] asmFileContent;
        List<Macro> macroList = [];
        foreach (SharedCode sharedCode in sharedCodes)
        {
            asmFileContent = ASMEditUtils
                .CleanFileContent(sharedCode.FilePath)
                .Split('\n');
            macroList.AddRange(readASMFile(asmFileContent, sharedCode));
        }
        return [.. macroList];
    }
    private static List<Macro> readASMFile(string[] asmFileContent, SharedCode sharedCode)
    {
        List<Macro> macroList = [];
        Match m;
        for (int i = 0; i < asmFileContent.Length; i++)
        {
            m = macroSignatureRegex().Match(asmFileContent[i]);
            if (m.Success)
                macroList.Add(new Macro(m.Groups["name"].Value, sharedCode, i + 1));
        }
        return macroList;
    }
    public static ValidationResult ValidateMacros(IEnumerable<Macro> macroList)
    {
        ValidationResult result = new();
        Dictionary<string, List<Macro>> dictionary = [];
        foreach (Macro macro in macroList)
        {
            if (!dictionary.ContainsKey(macro.Name))
                dictionary.Add(macro.Name, []);
            dictionary[macro.Name].Add(macro);
        }
        string macroArr;
        foreach (var kvp in dictionary.Where(entry => entry.Value.Count > 1))
        {
            macroArr = string.Join(".\n\t\t\t", [.. kvp.Value.Select(v => v.ToString())]);
            result.AddError(REPEATED_MACRO_NAME, new()
            {
                { "macros", macroArr }
            });
        }
        return result;
    }
}
