using OneOf;
using Validations;

namespace SMWHubEnumerators;
public class FileSection(string name, string filepath, int startLine, int endLine)
{
    public string FilePath { get; } = filepath;
    public string Name { get; } = name;
    public int StartLine { get; } = startLine > endLine ? endLine : startLine;
    public int EndLine { get; } = endLine;
    public static IEnumerable<OneOf<ValidationResult, FileSection>> GetSectionsFromFile(string filepath, ISet<string> sections, string? defaultSection = null, bool skipTitle = false, bool uniqueSections = true)
    {
        FileLineReader reader = new(filepath);
        HashSet<string> detectedSections = [];
        int commentIndex;
        string? currentSection = defaultSection;
        int currentSectionStart = 0;
        int currentLine = 0;
        string cleanLine;
        ValidationResult validation = new();
        int startAdder = skipTitle ? 1 : 0;
        foreach (var line in reader)
        {
            commentIndex = line.IndexOf(';');
            if (commentIndex < 0)
                commentIndex = line.Length;
            cleanLine = line[..commentIndex].Trim();
            if(!sections.Contains(cleanLine))
            {
                if (currentSection == null)
                    validation.AddError(new(filepath, currentLine + 1, cleanLine), SMWHubEnumeratorsMessageTypeKeys.SECTION_WITHOUT_TITLE);
                currentLine++;
                continue;
            }
            if(uniqueSections && detectedSections.Contains(cleanLine))
            {
                validation.AddError(new(filepath, currentLine + 1, cleanLine), 
                    SMWHubEnumeratorsMessageTypeKeys.REPEATED_SECTION, 
                    new Dictionary<string, string>
                    {
                        { "section", cleanLine }
                    });
                currentLine++;
                continue;
            }
            if(validation.IsValid)
            {
                if (currentSectionStart != currentLine)
                {
                    yield return new FileSection(currentSection!, filepath, currentSectionStart + startAdder, currentLine - 1);
                    detectedSections.Add(currentSection!);
                }
                currentSection = cleanLine;
                currentSectionStart = currentLine;
                detectedSections.Add(cleanLine);
            }
            currentLine++;
        }
        if (!validation.IsValid)
            yield return validation;
    }
    public FileLineEnumerator GetEnumerator()
    {
        FileLineReader reader = new(FilePath);
        return reader.GetLimitedEnumerator(startLine, EndLine);
    }
    public IEnumerable<string> Read()
    {
        FileLineReader reader = new(FilePath);

        foreach (var line in reader.ReadSection(StartLine, EndLine))
        {
            yield return line;
        }
    }
}
