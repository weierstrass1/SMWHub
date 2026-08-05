using Newtonsoft.Json;

namespace SMWHubSprites.Formats;

public class SpriteSP
{
    [JsonProperty("Value")]
    public int Value = 0x7F;
    [JsonProperty("Separate")]
    public bool Separate = false;
}
