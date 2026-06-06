using LogRegister;
using System.Collections;

namespace FormatReadLibrary.Logging.Enumerators;
public sealed class FileEnumeratorWithLog : IEnumerator<string>
{
    private readonly FileReaderWithLog _reader;
    public string Path => _reader.FilePath;
    public LogRegisterSystem Log => _reader.Log;
    public int LineIndex { get; private set; }
    public string Current
    {
        get
        {
            if (!IsValid())
                throw new InvalidOperationException();
            return _reader[LineIndex];
        }
    }
    object IEnumerator.Current => Current;
    private int _minLimit;
    private int _maxLimit;
    public FileEnumeratorWithLog(FileReaderWithLog reader)
    {
        _reader = reader;
        _minLimit = 0;
        _maxLimit = reader.Length - 1;
        LineIndex = -1;
    }
    public FileEnumeratorWithLog(FileReaderWithLog reader, int minLimit, int maxLimit)
    {
        _reader = reader;
        _minLimit = Math.Max(0, minLimit);
        _maxLimit = Math.Min(reader.Length - 1, maxLimit);
        LineIndex = _minLimit - 1;
    }
    public bool MoveNext()
    {
        LineIndex++;
        return LineIndex <= _maxLimit;
    }
    public void Reset()
    {
        LineIndex = _minLimit - 1;
    }
    public bool IsValid()
    {
        return LineIndex >= _minLimit && LineIndex <= _maxLimit;
    }
    public void Dispose()
    {
    }
    public bool IsLastLine()
    {
        return LineIndex == _maxLimit;
    }
}
