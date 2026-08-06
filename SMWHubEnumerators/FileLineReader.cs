using System.Collections;
using System.Text;

namespace SMWHubEnumerators;

public sealed class FileLineReader : IFormattedEnumerable
{
    public string FilePath { get; }
    public string? Format => throw new NotImplementedException();
    public string? Extension { get; }
    public int Length { get; private set; }
    public string this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (_foundLines.TryGetValue(index, out var line))
                return line;
            using FileStreamReader reader = new(FilePath);
            reader.Seek(_linePositions[index]);
            line = Encoding.UTF8.GetString([.. reader.ReadLine()]);
            _foundLines[index] = line;
            return line;
        }
    }
    private readonly Dictionary<int, string> _foundLines = [];
    private readonly List<long> _linePositions = [];
    public FileLineReader(string filepath)
    {
        FilePath = filepath;
        Extension = Path.GetExtension(filepath);
        getLinePositions();
    }
    public IEnumerable<string> Read()
    {
        foreach(var line in this)
            yield return line;
    }
    public IEnumerable<string> ReadSection(int start, int end)
    {
        using var en = GetLimitedEnumerator(start, end);
        while(en.MoveNext())
            yield return en.Current;
    }
    public FileLineEnumerator GetLimitedEnumerator(int start, int end)
    {
        start = Math.Clamp(start, 0, Length - 1);
        end = Math.Clamp(end, 0, Length - 1);
        if (start > end)
            start = end;
        return new(this, start, end);
    }
    public IEnumerator<string> GetEnumerator()
    {
        return new FileLineEnumerator(this, 0, Length - 1);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    internal long getPosition(int index)
    {
        if (index < 0 || index >= Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        return _linePositions[index];
    }
    private void getLinePositions()
    {
        using FileStreamReader reader = new(FilePath);
        Length = 0;
        while (!reader.EndOfFile)
        {
            _linePositions.Add(reader.CurrentPosition);
            reader.ReadLine();
            Length++;
        }
    }
}
