using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMWHubASMCodeLibrary.DTO;

public class FolderContainer
{
    [JsonRequired]
    public required string Main { get; set; }
    [JsonRequired]
    public required SpriteFolderContainer SpritesFolders { get; set; }
    [JsonRequired]
    public required string OverworldSpritesFolder { get; set; }
    [JsonRequired]
    public required UberasmFolderContainer UberasmFolder { get; set; }
    [JsonRequired]
    public required string BlocksFolder { get; set; }
    [JsonRequired]
    public required string PatchesFolder { get; set; }
    public static FolderContainer GetFromJson(string path)
    {
        if(!File.Exists(path))
            throw new FileNotFoundException();
        return JsonSerializer.Deserialize<FolderContainer>(File.ReadAllText(path))!;
    }
}
