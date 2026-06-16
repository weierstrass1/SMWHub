using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatLibrary.Entries
{
    public class NormalSpriteDisplayTile
    {
        [JsonProperty("X offset", Required = Required.Always)]
        public int XOffset;
        [JsonProperty("Y offset", Required = Required.Always)]
        public int YOffset;
        [JsonProperty("map16 tile", Required = Required.Always)]
        public int Map16Number;
    }
}
