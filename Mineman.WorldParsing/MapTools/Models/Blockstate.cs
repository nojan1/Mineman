using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.MapTools.Models
{
    [JsonObject]
    public class BlockStateModelSpecification
    {
        public string model { get; set; }
        public string y { get; set; }
    }

    public class BlockState
    {
        public Dictionary<string, BlockStateModelSpecification[]> variants { get; set; }
    }
}
