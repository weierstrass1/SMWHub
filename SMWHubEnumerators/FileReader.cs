using ASMCodeUtils;
using System.Collections;
using System.Text.RegularExpressions;
using Validations;

namespace SMWHubEnumerators;

public sealed class FileReader : IEnumerable<string>
{
    public string this[int index]
    {
        get => _fileContentLines[index];
    }
    public int Length => _fileContentLines.Length;
    public readonly string FilePath;
    private readonly string[] _fileContentLines;
    public FileReader(string path)
    {
        FilePath = path;
        _fileContentLines = ASMEditUtils.CleanFileContent(path).Split('\n');
    }
    public FileReader(string name, string content)
    {
        FilePath = Path.Combine("internal", name);
        _fileContentLines = ASMEditUtils.CleanString(content).Split('\n');
    }
    public IEnumerator<string> GetEnumerator()
    {
        return new FileEnumerator(this);
    }
    public bool SplitBySections(out Dictionary<string, FileEnumerator> enumerators, Regex regex, bool skipTitle = true)
    {
        return splitBySections(out enumerators,
            line => line,
            regex.IsMatch,
            line => regex.Match(line).Groups["id"].Value,
            skipTitle);
    }
    public bool SplitBySections(out Dictionary<string, FileEnumerator> enumerators, bool skipTitle = true, params string[] sections)
    {
        var lowerSections = sections.Select(s => s.ToLower().Trim()).ToHashSet();
        return splitBySections(out enumerators,
            line => line.ToLower(),
            lowerSections.Contains,
            line => line,
            skipTitle);
    }
    private ValidationResult splitBySections(out Dictionary<string, FileEnumerator> enumerators, Func<string, string> lineProcessing, Func<string, bool> match, Func<string, string> getID, bool skipTitle = true)
    {
        ValidationResult result = new();
        enumerators = [];
        int sectionStart = 0;
        string? section = null;
        string currentLine;
        int i;
        int lastNotEmptyLine = -1;
        string id;
        ValidationResult r;
        for (i = 0; i < Length; i++)
        {
            if (string.IsNullOrWhiteSpace(_fileContentLines[i]))
                continue;
            currentLine = lineProcessing(_fileContentLines[i]);
            if (!match(currentLine))
            {
                lastNotEmptyLine = i;
                continue;
            }
            id = getID(currentLine);
            r = validateSection(enumerators, section, lastNotEmptyLine, id);
            r.AddLine(i, _fileContentLines[i]);
            result.Merge(r);
            if (!r || 
                !tryAddEnumerator(enumerators, skipTitle, sectionStart, section, lastNotEmptyLine))
                continue;
            section = id;
            sectionStart = i;
            lastNotEmptyLine = -1;
        }
        currentLine = _fileContentLines[lastNotEmptyLine];
        id = getID(currentLine);
        r = validateSection(enumerators, section, lastNotEmptyLine, id);
        r.AddLine(i, _fileContentLines[i]);
        result.Merge(r);
        if (r)
            tryAddEnumerator(enumerators, skipTitle, sectionStart, section, lastNotEmptyLine);
        result.AddFile(FilePath);
        return result;
    }
    private ValidationResult validateSection(Dictionary<string, FileEnumerator> enumerators, 
        string? section, int lastNotEmptyLine, string id)
    {
        ValidationResult result = new();
        if (enumerators.ContainsKey(id))
            result.AddError(SMWHubEnumeratorsMessageTypeKeys.REPEATED_SECTION, new()
            {
                { "section", id }
            });
        
        if (lastNotEmptyLine < 0)
            return result;

        if (section == null)
            result.AddError(SMWHubEnumeratorsMessageTypeKeys.SECTION_WITHOUT_TITLE);
        return result;
    }
    private bool tryAddEnumerator(Dictionary<string, FileEnumerator> enumerators, bool skipTitle, int sectionStart, string? section, int lastNotEmptyLine)
    {
        if (section == null)
            return false;
        enumerators.Add(section, new FileEnumerator(this, sectionStart + (skipTitle ? 1 : 0), lastNotEmptyLine));
        return true;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
