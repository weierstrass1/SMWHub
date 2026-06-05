namespace LogRegister;

public interface ILoggingEntryWithNestedMessage : ILoggingEntry
{
    public IReadOnlyDictionary<string, ILoggingEntry> NestedEntries { get; }
}
