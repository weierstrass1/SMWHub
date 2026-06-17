using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class NormalSpriteSP
{
    [JsonProperty("Value")]
    public int Value = 0x7F;
    [JsonProperty("Separate")]
    public bool Separate = false;
}
