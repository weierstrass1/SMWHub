namespace LogRegister;

public interface ILoggingEntryWithNestedMessage : ILoggingEntry
{
    public IReadOnlyDictionary<string, ILoggingEntry> Entries { get; }
}
