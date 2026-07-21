using SMWHubASMCodeLibrary;
using SMWHubPatchBuilder.SingleCodePatchReferences;
using System.Text;
using System.Text.RegularExpressions;

namespace SMWHubPatchBuilder
{
    public static partial class SingleCodePatchGenerator
    {
        public const int CODE_POINTERS_ROM_ADDRESS = 0x01CD1E;
        public const string TEMPLATE_DIRECTORY = "Templates";
        public const string ASM_DIRECTORY = "ASM";
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
        public static string? GenerateSingleCode(int id, Code code)
        {
            ISingleCodePatchReference? codeRef = SingleCodePatchReferenceFactory.CreateInstance(code.Scope.Type);
            if (codeRef == null)
                return null;
            IEnumerable<string> labels = getLabelsInFile(code, codeRef);

            string content = File.ReadAllText(SINGLE_CODE_PATCH_TEMPLATE)
                .Replace("<address>", $"{CODE_POINTERS_ROM_ADDRESS:X6}")
                .Replace("<id>", $"{id:X6}")
                .Replace("<file>", code.FullPath);

            StringBuilder sb = new(content);
            if (labels.Any())
                sb.AppendLine();
            foreach(var label in labels)
            {
                sb.AppendLine($"print \"{char.ToUpperInvariant(label[0])}{label[1..].ToLower()} \",hex({label})");
            }
            return sb.ToString();
        }

        private static HashSet<string> getLabelsInFile(Code code, ISingleCodePatchReference codeRef)
        {
            if (codeRef.DetectedLabels.Length == 0)
                return [];

            Regex r = new($"({string.Join('|', codeRef.DetectedLabels)}):", RegexOptions.IgnoreCase);
            HashSet<string> labels = [];
            int namespaceCounter = 0;
            List<Match> matches = [];
            static int cmp(Match m1, Match m2)
            {
                if (m1.Index < m2.Index)
                    return -1;
                if (m1.Index > m2.Index)
                    return 1;
                return 0;
            }
            foreach (CodeLine line in code.ReadLines())
            {
                matches.AddRange(namespaceOffRegex().Matches(line.Content));
                matches.AddRange(namespaceRegex()
                    .Matches(line.Content)
                    .Where(m1 => !matches.Any(m2 => m2.Index == m1.Index)));
                matches.AddRange(r.Matches(line.Content));
                matches.Sort(cmp);

                foreach (Match m in matches)
                {
                    if (m.Groups["namespace"].Success)
                    {
                        namespaceCounter++;
                        continue;
                    }
                    if (m.Groups["off"].Success)
                    {
                        namespaceCounter--;
                        continue;
                    }
                    if (namespaceCounter == 0 && !labels.Contains(m.Value[..^1]))
                    {
                        labels.Add(m.Value[..^1]);
                        if (labels.Count == codeRef.DetectedLabels.Length)
                            return labels;
                    }
                }
                matches.Clear();
            }


            return labels;
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
                includeFile.AppendLine($"incsrc \"{Path.Combine(relativePath, scope.DirectoryPath, code.FilePath)}\"");
            }
            return includeFile.ToString();
        }

        [GeneratedRegex(@"(?<namespace>(\s|^)namespace (?!off$)[a-zA-Z][a-zA-Z\.0-9-_]*(\s|$))")]
        private static partial Regex namespaceRegex();
        [GeneratedRegex(@"(?<off>(\s|^)namespace off(\s|$))")]
        private static partial Regex namespaceOffRegex();
    }
}
