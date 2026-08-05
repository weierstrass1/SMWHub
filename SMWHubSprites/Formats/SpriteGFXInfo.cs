using Newtonsoft.Json;

namespace SMWHubSprites.Formats;

public class SpriteGFXInfo
{
    [JsonProperty("0")]
    public SpriteSP SP0 = new();
    [JsonProperty("1")]
    public SpriteSP SP1 = new();
    [JsonProperty("2")]
    public SpriteSP SP2 = new();
    [JsonProperty("3")]
    public SpriteSP SP3 = new();
}
