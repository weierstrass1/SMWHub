using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace SMWHubEnumerators;

public sealed class FileLineReader : IFormattedEnumerable, IDisposable
{
    private const int BUFFER_SIZE = 4096;
    public string FilePath { get; }
    public string? Format { get; } = null;
    public string? Extension { get; }
    public int Length { get; private set; }
    public string this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException($"Index {index} is out of range. Valid range is 0 to {Length - 1}.");
            seek(_linePositions[index]);
            return ReadLine()!;
        }
    }
    public bool EndOfFile { get; private set; } = false;
    private long _currentPosition = 0;
    private int _readBytes = BUFFER_SIZE;
    private int _readBufferLimit = BUFFER_SIZE;
    private readonly byte[] _readBuffer = new byte[BUFFER_SIZE];
    private readonly List<long> _linePositions;
    private readonly List<byte> _lineBuffer = [];
    private readonly FileStream _fileStream;
    public FileLineReader(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");
        FilePath = filePath;
        Extension = Path.GetExtension(filePath);
        _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        _linePositions = [];
        Length = 0;
        string? line;
        long pos;
        while (true)
        {
            pos = _currentPosition;
            line = ReadLine();
            if(line == null) 
                break;
            _linePositions.Add(pos);
            Length++;
        }
        seek(0);
    }
    public string? ReadLine()
    {
        if(EndOfFile)
            return null;

        _lineBuffer.Clear();
        byte b;
        while (true)
        {
            if(_readBytes >= _readBufferLimit)
            {
                if(_currentPosition >= _fileStream.Length)
                {
                    EndOfFile = true;
                    return _lineBuffer.Count > 0 ?
                        Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(_lineBuffer)) :
                        null;
                }
                _readBufferLimit = _fileStream.Read(_readBuffer, 0, BUFFER_SIZE);
                _readBytes = 0;
            }
            b = _readBuffer[_readBytes];
            _readBytes++;
            _currentPosition++;
            if (b == '\n')
                break;

            if (b != '\r')
                _lineBuffer.Add(b);
        }
        return Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(_lineBuffer));
    }
    public IEnumerator<string> GetEnumerator()
    {
        return new FileLineEnumerator(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public void Dispose()
    {
        _fileStream.Dispose();
    }
    private void seek(long position)
    {
        _fileStream.Position = position;
        _currentPosition = position;
        _readBytes = BUFFER_SIZE;
        _readBufferLimit = BUFFER_SIZE;
        EndOfFile = false;
    }
}
