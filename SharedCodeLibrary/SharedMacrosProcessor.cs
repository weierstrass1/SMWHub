using ASMCodeUtils;
using System.Text.RegularExpressions;

namespace SharedCodeLibrary;
public static partial class SharedMacrosProcessor
{
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
    private static IEnumerable<Macro> readASMFile(string[] asmFileContent, SharedCode sharedCode)
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
}
