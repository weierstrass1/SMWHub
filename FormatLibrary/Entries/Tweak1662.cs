using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class Tweak1662
{
    [JsonProperty("Sprite Clipping", Required = Required.Always)]
    public int SpriteClipping;
    [JsonProperty("Use shell as death frame", Required = Required.Always)]
    public bool UseShellAsADeathFrame;
    [JsonProperty("Fall straight down when killed", Required = Required.Always)]
    public bool FallsStraightDownWhenKilled;
    public byte Value
    {
        get
        {
            return (byte)(SpriteClipping |
                (UseShellAsADeathFrame ? 0x40 : 0) |
                (FallsStraightDownWhenKilled ? 0x80 : 0));
        }
        set
        {
            SpriteClipping = value & 0x3F;
            UseShellAsADeathFrame = (value & 0x40) != 0;
            FallsStraightDownWhenKilled = (value & 0x80) != 0;
        }
    }
}
