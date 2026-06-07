namespace LogRegister.Interfaces;

public interface ILoggingEntryWithNestedMessage : ILoggingEntry
{
    public IReadOnlyDictionary<string, ILoggingEntry> NestedEntries { get; }
}
