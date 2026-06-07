using LogRegister.Interfaces;

namespace LogRegister;

public enum SpanType
{
    Timestamp,
    Prefix,
    NormalText,
    Parameter,
    NestedMessage
}
public readonly struct LogSpan
{
    public readonly required ILogCategory Category { get; init; }
    public readonly required int Start { get; init; }
    public readonly required int Length { get; init; }
    public readonly required SpanType Type { get; init; }
    public LogSpan Displace(int offset)
    {
        return new()
        {
            Category = Category,
            Start = Start + offset,
            Length = Length,
            Type = Type
        };
    }
    public override string ToString()
    {
        return $"{Type}: ({Start},{Start + Length}) (size {Length})";
    }
}
