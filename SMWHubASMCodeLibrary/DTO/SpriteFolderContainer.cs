using System.Text.Json.Serialization;

namespace SMWHubASMCodeLibrary.DTO;

public class SpriteFolderContainer
{
    [JsonRequired]
    public required string Main { get; set; }
    [JsonRequired]
    public required string NormalSpritesFolder { get; set; }
    [JsonRequired]
    public required string ClusterSpritesFolder { get; set; }
    [JsonRequired]
    public required string ExtendedSpritesFolder { get; set; }
}
