using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Validations;

namespace SMWHubEnumerators;
public sealed class FileLineEnumerator : IEnumerator<string>
{
    public string FilePath => _reader.FilePath;
    public int LineIndex { get; private set; }
    public bool IsLastLine => LineIndex == _maxLimit;
    private string _current;
    public string Current
    {
        get
        {
            if (!IsValid())
                throw new InvalidOperationException();
            return _current;
        }
        set
        {
            _current = value;
        }
    }
    public ValidationContext Context => new(FilePath, LineIndex, Current);
    object IEnumerator.Current => Current;
    private int _minLimit;
    private int _maxLimit;
    private FileLineReader _reader;
    private FileStreamReader _fileStreamReader;
    public FileLineEnumerator(FileLineReader reader)
    {
        initialize(reader, 0, reader.Length - 1);
    }
    public FileLineEnumerator(FileLineReader reader, int minLimit, int maxLimit)
    {
        initialize(reader, minLimit, maxLimit);
    }
    public bool MoveNext()
    {
        if (LineIndex > _maxLimit)
            return false;

        LineIndex++;
        if (LineIndex > _maxLimit)
            return false;

        Current = Encoding.UTF8.GetString([.. _fileStreamReader.ReadLine()]);
        return true;
    }
    public void Reset()
    {
        LineIndex = _minLimit - 1;
        _fileStreamReader.Seek(_reader.getPosition(_minLimit));
    }
    public bool IsValid()
    {
        return LineIndex >= _minLimit && LineIndex <= _maxLimit;
    }
    public void Dispose()
    {
        _fileStreamReader.Dispose();
    }
    [MemberNotNull(nameof(_reader), nameof(_fileStreamReader), nameof(_current))]
    private void initialize(FileLineReader reader, int minLimit, int maxLimit)
    {
        _current = "";
        _reader = reader;
        _minLimit = minLimit;
        _maxLimit = maxLimit;
        LineIndex = -1;
        _fileStreamReader = new(reader.FilePath);
        Reset();
    }
}
