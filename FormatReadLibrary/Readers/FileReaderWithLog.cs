using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using static System.Collections.Specialized.BitVector32;

namespace FormatReadLibrary.Readers;

public sealed class FileReaderWithLog : IEnumerable<string>
{
    public string this[int index]
    {
        get => _fileContentLines[index];
    }
    public int Length => _fileContentLines.Length;
    public readonly LogRegisterSystem Log;
    private readonly string _path;
    private readonly string[] _fileContentLines;
    public FileReaderWithLog(string path, LogRegisterSystem log)
    {
        Log = log;
        _path = path;
        _fileContentLines = FileUtils.CleanFileContent(path).Split('\n');
    }
    public FileReaderWithLog(string name, string content, LogRegisterSystem log)
    {
        Log = log;
        _path = Path.Combine("internal", name);
        _fileContentLines = FileUtils.CleanString(content).Split('\n');
    }
    public void AddLog(int i ,Func<int, string, string, ILoggingRegister> registerFunc)
    {
        Log.Add(registerFunc(i, _path, _fileContentLines[i]));
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
        bool notEmptyLine = false;
        int i;
        string id;
        bool emptySection = true;
        for (i = 0; i < Length; i++)
        {
            if (string.IsNullOrWhiteSpace(_fileContentLines[i]))
                continue;
            currentLine = lineProcessing(_fileContentLines[i]);
            if (!match(currentLine))
            {
                notEmptyLine = true;
                if (section != null)
                    emptySection = false;
                continue;
            }
            if(!processSection(enumerators, getID, emptySection, sectionStart, section, currentLine, notEmptyLine, i, out id, skipTitle))
                return false;
            section = id;
            sectionStart = i;
            notEmptyLine = true;
            emptySection = true;
        }
        currentLine = _fileContentLines[^1];
        i = Length - 1;
        return processSection(enumerators, getID, emptySection, sectionStart, section, currentLine, notEmptyLine, i, out id, skipTitle);
    }
    private bool processSection(Dictionary<string, FileEnumeratorWithLog> enumerators, Func<string, string> getID, 
        bool emptySection, int sectionStart, string? section, 
        string currentLine, bool notEmptyLine, int i, out string id, bool skipTitle)
    {
        id = getID(currentLine);
        if (enumerators.ContainsKey(id))
        {
            Log.Add(new SyntaxError(i, _path, currentLine, $"Repeated Section {id}"));
            return false;
        }
        if (section == null && notEmptyLine)
        {
            Log.Add(new SyntaxError(i, _path, currentLine, "\"Section doesn't contain title\""));
            return false;
        }
        if (section != null && !emptySection)
            enumerators.Add(section, new FileEnumeratorWithLog(this, sectionStart + (skipTitle ? 1 : 0), i - 1));
        return true;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
