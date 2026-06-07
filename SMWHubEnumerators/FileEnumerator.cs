using System.Collections;
using Validations;

namespace SMWHubEnumerators;
public sealed class FileEnumerator : IEnumerator<string>
{
    private readonly FileReader _reader;
    public string FilePath => _reader.FilePath;
    public int LineIndex { get; private set; }
    public bool IsLastLine => LineIndex == _maxLimit;
    public string Current
    {
        get
        {
            if (!IsValid())
                throw new InvalidOperationException();
            return _reader[LineIndex];
        }
    }
    public ValidationContext Context => new(FilePath, LineIndex, Current);
    object IEnumerator.Current => Current;
    private readonly int _minLimit;
    private readonly int _maxLimit;
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
}
