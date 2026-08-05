using Newtonsoft.Json;

namespace SMWHubSprites.Formats;

public class Tweak167A
{
    [JsonProperty("Don't disable cliping when starkilled", Required = Required.Always)]
    public bool DontDisableClippingWhenStartKilled;
    [JsonProperty("Invincible to star/cape/fire/bounce blk.", Required = Required.Always)]
    public bool InvincibleToStarCapeFireBouncingBLK;
    [JsonProperty("Process when off screen", Required = Required.Always)]
    public bool ProcessOutOfScreen;
    [JsonProperty("Don't change into shell when stunned", Required = Required.Always)]
    public bool DontChangeIntoAShellWhenStunned;
    [JsonProperty("Can't be kicked like shell", Required = Required.Always)]
    public bool CantBeKickedLikeAShell;
    [JsonProperty("Process interaction with Mario every frame", Required = Required.Always)]
    public bool ProcessInteractionWithMarioEveryFrame;
    [JsonProperty("Gives power-up when eaten by yoshi", Required = Required.Always)]
    public bool GivesPowerUpWhenEatenByYoshi;
    [JsonProperty("Don't use default interaction with Mario", Required = Required.Always)]
    public bool DontUseDefaultInteractionWithMario;
    public byte Value
    {
        get
        {
            return (byte)((DontDisableClippingWhenStartKilled ? 0x1 : 0) |
                (InvincibleToStarCapeFireBouncingBLK ? 0x2 : 0) |
                (ProcessOutOfScreen ? 0x4 : 0) |
                (DontChangeIntoAShellWhenStunned ? 0x8 : 0) |
                (CantBeKickedLikeAShell ? 0x10 : 0) |
                (ProcessInteractionWithMarioEveryFrame ? 0x20 : 0) |
                (GivesPowerUpWhenEatenByYoshi ? 0x40 : 0) |
                (DontUseDefaultInteractionWithMario ? 0x80 : 0));
        }
        set
        {
            DontDisableClippingWhenStartKilled = (value & 0x1) != 0;
            InvincibleToStarCapeFireBouncingBLK = (value & 0x2) != 0;
            ProcessOutOfScreen = (value & 0x4) != 0;
            DontChangeIntoAShellWhenStunned = (value & 0x8) != 0;
            CantBeKickedLikeAShell = (value & 0x10) != 0;
            ProcessInteractionWithMarioEveryFrame = (value & 0x20) != 0;
            GivesPowerUpWhenEatenByYoshi = (value & 0x40) != 0;
            DontUseDefaultInteractionWithMario = (value & 0x80) != 0;
        }
    }
    public static implicit operator Tweak167A(byte b)
    {
        return new()
        {
            Value = b
        };
    }
}
