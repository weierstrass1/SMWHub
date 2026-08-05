using Newtonsoft.Json;

namespace SMWHubSprites.Formats;

public class SpriteConfigEntry
{
    [JsonIgnore]
    public string? CFGPath;
    [JsonIgnore]
    public int ID;
    [JsonProperty("AsmFile", Required = Required.Always)]
    public required string Filepath;
    [JsonProperty("ActLike", Required = Required.Always)]
    public byte ActLike;
    [JsonProperty("Extra Property Byte 1", Required = Required.Always)]
    public byte ExtraPropertyByte1;
    [JsonProperty("Extra Property Byte 2", Required = Required.Always)]
    public byte ExtraPropertyByte2;
    [JsonProperty("Collection")]
    public List<SpritePrefab> Prefabs = [];

    [JsonProperty("Additional Byte Count (extra bit clear)", Required = Required.Always)]
    public int ExtraBytesWithClearExtraBit;
    [JsonProperty("Additional Byte Count (extra bit set)", Required = Required.Always)]
    public int ExtraBytesWithSetExtraBit;

    [JsonProperty("$1656", Required = Required.Always)]
    public Tweak1656 Tweak1656 = new();
    [JsonProperty("$1662", Required = Required.Always)]
    public Tweak1662 Tweak1662 = new();
    [JsonProperty("$166E", Required = Required.Always)]
    public Tweak166E Tweak166E = new();
    [JsonProperty("$167A", Required = Required.Always)]
    public Tweak167A Tweak167A = new();
    [JsonProperty("$1686", Required = Required.Always)]
    public Tweak1686 Tweak1686 = new();
    [JsonProperty("$190F", Required = Required.Always)]
    public Tweak190F Tweak190F = new();
    [JsonProperty("DisplayType", Required = Required.Always)]
    public string DisplayType = "ExByte";
    [JsonProperty("Map16")]
    public string Map16 = "";
    [JsonProperty("Type")]
    public int Type = 1;
}
