using Newtonsoft.Json;

namespace SMWHubSprites.Formats;

public class SpriteDisplay
{
    [JsonProperty("Description")]
    public string Description = "";
    [JsonProperty("DisplayText")]
    public string DisplayText = "";
    [JsonProperty("ExtraBit")]
    public bool ExtraBit;
    [JsonProperty("GFXInfo")]
    public SpriteGFXInfo GFXInfo = new();
    [JsonProperty("Index")]
    public int Index;
    [JsonProperty("Tiles")]
    public List<SpriteDisplayTile> Tiles = [];
    [JsonProperty("UseText")]
    public bool UseText;
    [JsonProperty("Value")]
    public int Value;
}
