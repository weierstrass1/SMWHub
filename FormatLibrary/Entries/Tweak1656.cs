using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class Tweak1656 : TweakNumber
{
    [JsonProperty("Object Clipping", Required = Required.Always)]
    public int ObjectClipping;
    [JsonProperty("Can be jumped on", Required = Required.Always)]
    public bool CanBeJumpedOn;
    [JsonProperty("Dies when jumped on", Required = Required.Always)]
    public bool DiesWhenJumpedOn;
    [JsonProperty("Hop in/kick shell", Required = Required.Always)]
    public bool HopInKickShells;
    [JsonProperty("Disappears in cloud of smoke", Required = Required.Always)]
    public bool DissappearInACloudOfSmoke;
    public override byte Value
    {
        get
        {
            return (byte)(ObjectClipping |
                (CanBeJumpedOn ? 0x10 : 0) |
                (DiesWhenJumpedOn ? 0x20 : 0) |
                (HopInKickShells ? 0x40 : 0) |
                (DissappearInACloudOfSmoke ? 0x80 : 0));
        }
        set
        {
            ObjectClipping = value & 0xF;
            CanBeJumpedOn = (value & 0x10) != 0;
            DiesWhenJumpedOn = (value & 0x20) != 0;
            HopInKickShells = (value & 0x40) != 0;
            DissappearInACloudOfSmoke = (value & 0x80) != 0;
        }
    }
    public static implicit operator Tweak1656(byte b)
    {
        return new()
        {
            Value = b
        };
    }
}
