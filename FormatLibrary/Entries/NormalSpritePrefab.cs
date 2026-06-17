using Newtonsoft.Json;

namespace FormatLibrary.Entries;

public class NormalSpritePrefab
{
    [JsonProperty("Name", Required = Required.Always)]
    public required string Name;
    [JsonProperty("ExtraBit")]
    public bool Extrabit;
    [JsonProperty("Extra Property Byte 1")]
    public byte ExtraByte1;
    [JsonProperty("Extra Property Byte 2")]
    public byte ExtraByte2;
    [JsonProperty("Extra Property Byte 3")]
    public byte ExtraByte3;
    [JsonProperty("Extra Property Byte 4")]
    public byte ExtraByte4;
    [JsonProperty("Extra Property Byte 5")]
    public byte ExtraByte5;
    [JsonProperty("Extra Property Byte 6")]
    public byte ExtraByte6;
    [JsonProperty("Extra Property Byte 7")]
    public byte ExtraByte7;
    [JsonProperty("Extra Property Byte 8")]
    public byte ExtraByte8;
    [JsonProperty("Extra Property Byte 9")]
    public byte ExtraByte9;
    [JsonProperty("Extra Property Byte 10")]
    public byte ExtraByte10;
    [JsonProperty("Extra Property Byte 11")]
    public byte ExtraByte11;
    [JsonProperty("Extra Property Byte 12")]
    public byte ExtraByte12;
}
