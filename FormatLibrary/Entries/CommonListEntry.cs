using FormatReadLibrary.Interfaces;

namespace FormatLibrary.Entries;

public sealed class CommonListEntry
{
    public required ICommonListCategory Category { get; init; }
    public required int ID { get; init; }
    public required FilePath[] Paths { get; init; }
}
