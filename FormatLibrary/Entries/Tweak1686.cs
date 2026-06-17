using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class Tweak1686 : TweakNumber
{
    [JsonProperty("Inedible", Required = Required.Always)]
    public bool Inedible;
    [JsonProperty("Stay in Yoshi's mouth", Required = Required.Always)]
    public bool StayInYoshisMouth;
    [JsonProperty("Weird ground behaviour", Required = Required.Always)]
    public bool WeirdGroundBehaviour;
    [JsonProperty("Don't interact with other sprites", Required = Required.Always)]
    public bool DontInteractWithOtherSprites;
    [JsonProperty("Don't change direction if touched", Required = Required.Always)]
    public bool DontChangeDirectionIfTouched;
    [JsonProperty("Don't turn into coin when goal passed", Required = Required.Always)]
    public bool DontTurnIntoCoinWhenGoalPassed;
    [JsonProperty("Spawn a new sprite", Required = Required.Always)]
    public bool SpawnsANewSprite;
    [JsonProperty("Don't interact with objects", Required = Required.Always)]
    public bool DontInteractWithObjects;
    public override byte Value
    {
        get
        {
            return (byte)((Inedible ? 0x1 : 0) |
                (StayInYoshisMouth ? 0x2 : 0) |
                (WeirdGroundBehaviour ? 0x4 : 0) |
                (DontInteractWithOtherSprites ? 0x8 : 0) |
                (DontChangeDirectionIfTouched ? 0x10 : 0) |
                (DontTurnIntoCoinWhenGoalPassed ? 0x20 : 0) |
                (SpawnsANewSprite ? 0x40 : 0) |
                (DontInteractWithObjects ? 0x80 : 0));
        }
        set
        {
            Inedible = (value & 0x1) != 0;
            StayInYoshisMouth = (value & 0x2) != 0;
            WeirdGroundBehaviour = (value & 0x4) != 0;
            DontInteractWithOtherSprites = (value & 0x8) != 0;
            DontChangeDirectionIfTouched = (value & 0x10) != 0;
            DontTurnIntoCoinWhenGoalPassed = (value & 0x20) != 0;
            SpawnsANewSprite = (value & 0x40) != 0;
            DontInteractWithObjects = (value & 0x80) != 0;
        }
    }
    public static implicit operator Tweak1686(byte b)
    {
        return new()
        {
            Value = b
        };
    }
}
