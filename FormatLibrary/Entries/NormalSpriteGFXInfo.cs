using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class NormalSpriteGFXInfo
{
    [JsonProperty("0")]
    public NormalSpriteSP SP0 = new();
    [JsonProperty("1")]
    public NormalSpriteSP SP1 = new();
    [JsonProperty("2")]
    public NormalSpriteSP SP2 = new();
    [JsonProperty("3")]
    public NormalSpriteSP SP3 = new();
}
