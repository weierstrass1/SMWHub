using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class Tweak190F
{
    [JsonProperty("Make platform passable from below", Required = Required.Always)]
    public bool MakePlatformPassableFromBelow;
    [JsonProperty("Don't erase when goal passed", Required = Required.Always)]
    public bool DontEraseWhenGoalPassed;
    [JsonProperty("Can't be killed by sliding", Required = Required.Always)]
    public bool CantBeKilledBySliding;
    [JsonProperty("Takes 5 fireballs to kill", Required = Required.Always)]
    public bool Takes5FireballsToKill;
    [JsonProperty("Can be jumped on with upwards Y speed", Required = Required.Always)]
    public bool CanBeJumpedOnWithUpwardYSpeed;
    [JsonProperty("Death frame two tiles high", Required = Required.Always)]
    public bool DeathFrame2TilesHigh;
    [JsonProperty("Don't turn into a coin with silver POW", Required = Required.Always)]
    public bool DontTurnIntoCoinWithSilverPOW;
    [JsonProperty("Don't get stuck in walls (carryable sprites)", Required = Required.Always)]
    public bool DontGetStuckInWalls_CarriableSprites;
    public byte Value
    {
        get
        {
            return (byte)((MakePlatformPassableFromBelow ? 0x1 : 0) |
                (DontEraseWhenGoalPassed ? 0x2 : 0) |
                (CantBeKilledBySliding ? 0x4 : 0) |
                (Takes5FireballsToKill ? 0x8 : 0) |
                (CanBeJumpedOnWithUpwardYSpeed ? 0x10 : 0) |
                (DeathFrame2TilesHigh ? 0x20 : 0) |
                (DontTurnIntoCoinWithSilverPOW ? 0x40 : 0) |
                (DontGetStuckInWalls_CarriableSprites ? 0x80 : 0));
        }
        set
        {
            MakePlatformPassableFromBelow = (value & 0x1) != 0;
            DontEraseWhenGoalPassed = (value & 0x2) != 0;
            CantBeKilledBySliding = (value & 0x4) != 0;
            Takes5FireballsToKill = (value & 0x8) != 0;
            CanBeJumpedOnWithUpwardYSpeed = (value & 0x10) != 0;
            DeathFrame2TilesHigh = (value & 0x20) != 0;
            DontTurnIntoCoinWithSilverPOW = (value & 0x40) != 0;
            DontGetStuckInWalls_CarriableSprites = (value & 0x80) != 0;
        }
    }
}
