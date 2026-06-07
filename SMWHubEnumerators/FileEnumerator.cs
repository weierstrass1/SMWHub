using System.Collections;

namespace SMWHubEnumerators;
public sealed class FileEnumerator : IEnumerator<string>
{
    private readonly FileReader _reader;
    public string Path => _reader.FilePath;
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
    public FileEnumerator(FileReader reader)
    {
        _reader = reader;
        _minLimit = 0;
        _maxLimit = reader.Length - 1;
        LineIndex = -1;
    }
    public FileEnumerator(FileReader reader, int minLimit, int maxLimit)
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
