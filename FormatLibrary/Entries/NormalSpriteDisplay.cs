using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatLibrary.Entries;

public class NormalSpriteDisplay
{
    [JsonProperty("Description")]
    public string Description = "";
    [JsonProperty("DisplayText")]
    public string DisplayText = "";
    [JsonProperty("ExtraBit")]
    public bool ExtraBit;
    [JsonProperty("GFXInfo")]
    public NormalSpriteGFXInfo GFXInfo = new();
    [JsonProperty("Index")]
    public int Index;
    [JsonProperty("Tiles")]
    public List<NormalSpriteDisplayTile> Tiles = [];
    [JsonProperty("UseText")]
    public bool UseText;
    [JsonProperty("Value")]
    public int Value;
}
