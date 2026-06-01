using LogRegister;
using System.Collections;
using System.IO;

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
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
