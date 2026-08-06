using System.Text;

namespace SMWHubEnumerators;

public class FileStreamReader : IDisposable
{
    private const int BUFFER_SIZE = 4096;
    public bool EndOfFile { get; private set; } = false;
    public long CurrentPosition { get; private set; } = 0;
    public long Length => _fileStream.Length;
    private int _readBytes = BUFFER_SIZE;
    private int _readBufferLimit = BUFFER_SIZE;
    private readonly byte[] _readAheadBuffer = new byte[BUFFER_SIZE];
    private readonly FileStream _fileStream;
    private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
    private readonly char[] _charBuffer = new char[2];
    private int _charBufferIndex = 0;
    private int _charBufferLength = 0;
    private readonly byte[] _byteBuffer = new byte[1];
    public FileStreamReader(string filepath)
    {
        if (!File.Exists(filepath))
            throw new FileNotFoundException($"The file '{filepath}' does not exist.");
        _fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        EndOfFile = _fileStream.Length == 0;
    }
    public void Seek(long position)
    {
        if(_fileStream.Length == 0) 
            return;
        if (position == CurrentPosition)
            return;
        position = Math.Clamp(position, 0, _fileStream.Length);
        _decoder.Reset();
        _charBufferIndex = 0;
        _charBufferLength = 0;
        if (position == _fileStream.Length)
        {
            CurrentPosition = position;
            _readBytes = BUFFER_SIZE;
            _readBufferLimit = BUFFER_SIZE;
            _fileStream.Position = position;
            EndOfFile = true;
            return;
        }
        EndOfFile = false;
        long buffStart = _fileStream.Position - _readBufferLimit;
        if (buffStart <= position && _fileStream.Position > position)
        {
            CurrentPosition = position;
            _readBytes = (int)(position - buffStart);
            return;
        }
        CurrentPosition = position;
        _readBytes = BUFFER_SIZE;
        _readBufferLimit = BUFFER_SIZE;
        _fileStream.Position = position;
    }
    public byte? ReadByte()
    {
        if (_readBytes >= _readBufferLimit)
        {
            if (CurrentPosition >= _fileStream.Length)
            {
                EndOfFile = true;
                return null;
            }
            _readBufferLimit = _fileStream.Read(_readAheadBuffer, 0, BUFFER_SIZE);
            _readBytes = 0;
            if (CurrentPosition == 0 && _fileStream.Length > 2 &&
                _readAheadBuffer[0] == 0xEF &&
                _readAheadBuffer[1] == 0xBB &&
                _readAheadBuffer[2] == 0xBF)
            {
                _readBytes = 3;
                CurrentPosition = 3;
            }
        }
        byte b = _readAheadBuffer[_readBytes];
        _readBytes++;
        CurrentPosition++;
        return b;
    }
    public char? ReadChar()
    {
        if (_charBufferIndex < _charBufferLength)
            return _charBuffer[_charBufferIndex++];

        while (true)
        {
            byte? b = ReadByte();

            if (b == null)
                return null;

            _byteBuffer[0] = b.Value;

            _charBufferLength = _decoder.GetChars(
                _byteBuffer.AsSpan(0, 1),
                _charBuffer.AsSpan(),
                flush: false);

            if (_charBufferLength == 0)
                continue;

            _charBufferIndex = 1;
            return _charBuffer[0];
        }
    }
    public IEnumerable<byte> ReadLine()
    {
        byte? b;
        while (true)
        {
            b = ReadByte();
            if (b == null || b == '\n')
                yield break;
            if (b != '\r')
                yield return b.Value;
        }
    }
    public IEnumerable<byte> Read(Func<byte, bool> predicate)
    {
        byte? b;
        while(true)
        {
            b = ReadByte();
            if (b == null)
                yield break;
            yield return b.Value;
            if (predicate(b.Value))
                yield break;
        }
    }
    public IEnumerable<byte> Read()
    {
        byte? b;
        while (true)
        {
            b = ReadByte();
            if (b == null)
                yield break;
            yield return b.Value;
        }
    }
    public void Dispose()
    {
        _decoder.Reset();
        _fileStream.Dispose();
    }
}
