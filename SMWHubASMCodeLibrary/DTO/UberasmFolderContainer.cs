using System.Text.Json.Serialization;

namespace SMWHubASMCodeLibrary.DTO;

public class UberasmFolderContainer
{
    [JsonRequired]
    public required string Main { get; set; }
    [JsonRequired]
    public required string LevelFolder { get; set; }
    [JsonRequired]
    public required string GameModeFolder { get; set; }
    [JsonRequired]
    public required string OverworldFolder { get; set; }
}
