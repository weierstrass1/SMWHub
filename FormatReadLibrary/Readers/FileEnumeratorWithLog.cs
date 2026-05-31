using LogRegister;
using System.Collections;

namespace FormatReadLibrary.Readers;
public sealed class FileEnumeratorWithLog(FileReaderWithLog reader) : IEnumerator<string>
{
    private readonly FileReaderWithLog _reader = reader;
    public LogRegisterSystem Log => _reader.Log;
    public int LineIndex { get; private set; } = -1;
    public string Current
    {
        get
        {
            if (LineIndex < 0 || LineIndex >= _reader.Length)
                throw new InvalidOperationException();
            return _reader[LineIndex];
        }
    }
    object IEnumerator.Current => Current;
    public void AddLog(Func<int, string, string, ILoggingRegister> registerFunc)
    {
        _reader.AddLog(LineIndex, registerFunc);
    }
    public bool MoveNext()
    {
        LineIndex++;
        return LineIndex < _reader.Length;
    }
    public void Reset()
    {
        LineIndex = -1;
    }
    public void Dispose()
    {
    }
}
