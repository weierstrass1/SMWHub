using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using System.Collections;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Logging.Enumerators;

public sealed class FileReaderWithLog : IEnumerable<string>
{
    public string this[int index]
    {
        get => _fileContentLines[index];
    }
    public int Length => _fileContentLines.Length;
    public readonly LogRegisterSystem Log;
    public readonly string Path;
    private readonly string[] _fileContentLines;
    public FileReaderWithLog(string path, LogRegisterSystem log)
    {
        Log = log;
        Path = path;
        _fileContentLines = FileUtils.CleanFileContent(path).Split('\n');
    }
    public FileReaderWithLog(string name, string content, LogRegisterSystem log)
    {
        Log = log;
        Path = System.IO.Path.Combine("internal", name);
        _fileContentLines = FileUtils.CleanString(content).Split('\n');
    }
    public IEnumerator<string> GetEnumerator()
    {
        return new FileEnumeratorWithLog(this);
    }
    public bool SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, Regex regex, bool skipTitle = true)
    {
        return splitBySections(out enumerators,
            line => line,
            line => regex.IsMatch(line),
            line => regex.Match(line).Groups["id"].Value,
            skipTitle);
    }
    public bool SplitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, bool skipTitle = true, params string[] sections)
    {
        var lowerSections = sections.Select(s => s.ToLower().Trim()).ToHashSet();
        return splitBySections(out enumerators,
            line => line.ToLower(),
            line => lowerSections.Contains(line),
            line => line,
            skipTitle);
    }
    private bool splitBySections(out Dictionary<string, FileEnumeratorWithLog> enumerators, Func<string, string> lineProcessing, Func<string, bool> match, Func<string, string> getID, bool skipTitle = true)
    {
        enumerators = [];
        int sectionStart = 0;
        string? section = null;
        string currentLine;
        int i;
        int lastNotEmptyLine = -1;
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
            if(!processSection(enumerators, getID, sectionStart, section, currentLine, lastNotEmptyLine, i, out string id, skipTitle))
                return false;
            section = id;
            sectionStart = i;
            lastNotEmptyLine = -1;
        }
        currentLine = _fileContentLines[lastNotEmptyLine];
        return processSection(enumerators, getID, sectionStart, section, currentLine, lastNotEmptyLine, lastNotEmptyLine, out _, skipTitle);
    }
    private bool processSection(Dictionary<string, FileEnumeratorWithLog> enumerators, Func<string, string> getID, 
        int sectionStart, string? section, string currentLine, int lastNotEmptyLine, 
        int i, out string id, bool skipTitle)
    {
        id = getID(currentLine);
        if (enumerators.ContainsKey(id))
        {
            Log.Add(new SyntaxError(i, Path, currentLine, $"Repeated Section {id}"));
            return false;
        }
        if (section == null && lastNotEmptyLine >= 0)
        {
            Log.Add(new SyntaxError(i, Path, currentLine, "\"Section doesn't contain title\""));
            return false;
        }
        if (section != null && lastNotEmptyLine >= 0)
            enumerators.Add(section, new FileEnumeratorWithLog(this, sectionStart + (skipTitle ? 1 : 0), lastNotEmptyLine));
        return true;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
