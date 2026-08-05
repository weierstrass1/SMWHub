namespace SMWHubEnumerators;
/// <summary>
/// Represents an enumerable of strings that has a specific format or file extension. This interface is used to provide additional information about the format and extension of the strings in the enumerable, which can be useful for processing and handling the data in a consistent manner.
/// </summary>
public interface IFormattedEnumerable: IEnumerable<string>
{
    /// <summary>
    /// Gets the format of the strings in the enumerable. This can be used to determine how to process and handle the data in a consistent manner.
    /// </summary>
    public string? Format { get; }
    /// <summary>
    /// Gets the file extension of the strings in the enumerable. This can be used to determine how to process and handle the data in a consistent manner.
    /// </summary>
    public string? Extension { get; }
}
