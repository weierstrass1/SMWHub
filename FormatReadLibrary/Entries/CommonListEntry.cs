namespace FormatReadLibrary.Entries;
public sealed class CommonListEntry
{
    public required string EntryType { get; init; }
    public required int ID { get; init; }
    public required string Path { get; init; }
    public required int[] Values { get; init; }
    public override string ToString()
    {
        return $"{EntryType}-{ID:X2}-{Path}" + (Values.Length != 0 ?
            $"-[{string.Join(",", Values)}]" :
            "");
    }
}
