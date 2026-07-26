using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMWHubASMCodeLibrary.DTO;

public class FolderContainer
{
    [JsonRequired]
    [JsonInclude]
    public required string Main { get; init; }
    [JsonRequired]
    [JsonInclude]
    public required SpriteFolderContainer SpritesFolders { get; init; }
    [JsonRequired]
    [JsonInclude]
    public required string OverworldSpritesFolder { get; init; }
    [JsonRequired]
    [JsonInclude]
    public required UberasmFolderContainer UberasmFolder { get; init; }
    [JsonRequired]
    [JsonInclude]
    public required string BlocksFolder { get; init; }
    [JsonRequired]
    [JsonInclude]
    public required string PatchesFolder { get; init; }
    public static FolderContainer GetFromJson(string path)
    {
        if(!File.Exists(path))
            throw new FileNotFoundException();
        return JsonSerializer.Deserialize<FolderContainer>(File.ReadAllText(path))!;
    }
}
