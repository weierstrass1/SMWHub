using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatLibrary.Entries
{
    public class NormalSpriteSP
    {
        [JsonProperty("Value")]
        public int Value = 0x7F;
        [JsonProperty("Separate")]
        public bool Separate = false;
    }
}
