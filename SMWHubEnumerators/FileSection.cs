using OneOf;
using System.Collections;
using Validations;

namespace SMWHubEnumerators;
public class FileSection(string name, string filepath, int startLine, int endLine) : IFormattedEnumerable
{
    public string? Format { get; init; }
    public string? Extension { get; init; } = Path.GetExtension(filepath);
    public string FilePath { get; } = filepath;
    public string Name { get; } = name;
    public int StartLine { get; } = startLine > endLine ? endLine : startLine;
    public int EndLine { get; } = endLine;
    public static IEnumerable<OneOf<ValidationResult, FileSection>> GetSectionsFromFile(string filepath, IEnumerable<string> sections, string? defaultSection = null, bool skipTitle = false, bool uniqueSections = true)
    {
        HashSet<string> sectionSet = [.. sections.Select(s => s.ToLowerInvariant())];
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
            cleanLine = line[..commentIndex].Trim().ToLowerInvariant();
            if(!sectionSet.Contains(cleanLine))
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
    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        return GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
