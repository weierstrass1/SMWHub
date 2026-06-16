namespace FormatLibrary.Entries;
public sealed class CommonListEntry
{
    public string? EntryType { get; init; }
    public required int ID { get; init; }
    public required FilePath[] Paths { get; init; }
}
