namespace FormatLibrary.Entries;

public sealed class GPSListEntry
{
    public required int ID { get; init; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public required string Path { get; init; }
    public int ActLike { get; set; } = -1;
    public int[]? Values { get; set; } = [];
    public override string ToString()
    {
        string actlike = ActLike >= 0 ? $"-{ActLike:X4}" : "";
        return $"{ID:X4}-{Path}{actlike}";
    }
}
