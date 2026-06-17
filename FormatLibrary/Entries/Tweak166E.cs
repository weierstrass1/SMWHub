using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class Tweak166E
{
    [JsonProperty("Use second graphics page", Required = Required.Always)]
    public bool UseSecondPageOfGraphics;
    [JsonProperty("Palette", Required = Required.Always)]
    public int Palette;
    [JsonProperty("Disable fireball killing", Required = Required.Always)]
    public bool DisableFireballKilling;
    [JsonProperty("Disable cape killing", Required = Required.Always)]
    public bool DisableCapeKilling;
    [JsonProperty("Disable water splash", Required = Required.Always)]
    public bool DisableWaterSplash;
    [JsonProperty("Don't interact with Layer 2", Required = Required.Always)]
    public bool DontInteractWithLayer2;
    public byte Value
    {
        get
        {
            return (byte)((UseSecondPageOfGraphics ? 0x1 : 0) |
                (Palette << 1) |
                (DisableFireballKilling ? 0x10 : 0) |
                (DisableCapeKilling ? 0x20 : 0) |
                (DisableWaterSplash ? 0x40 : 0) |
                (DontInteractWithLayer2 ? 0x80 : 0));
        }
        set
        {
            UseSecondPageOfGraphics = (value & 0x1) != 0;
            Palette = (value & 0xE) >> 1;
            DisableFireballKilling = (value & 0x10) != 0;
            DisableCapeKilling = (value & 0x20) != 0;
            DisableWaterSplash = (value & 0x40) != 0;
            DontInteractWithLayer2 = (value & 0x80) != 0;
        }
    }
    public static implicit operator byte(Tweak166E t)
    {
        return t.Value;
    }
    public static implicit operator Tweak166E(byte b)
    {
        return new()
        {
            Value = b
        };
    }
}
