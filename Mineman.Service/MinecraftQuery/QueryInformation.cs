using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Service.MinecraftQuery
{
    public class QueryInformation
    {
        public Dictionary<string, string> ResponseFields { get; private set; } = new Dictionary<string, string>();
        public ICollection<PlayerInformation> Players { get; set; } = new List<PlayerInformation>();
        public ICollection<PluginInformation> Plugins { get; set; } = new List<PluginInformation>();

        public int MaxPlayers
        {
            get
            {
                return ResponseFields.ContainsKey("maxplayers") ? Convert.ToInt32(ResponseFields["maxplayers"]) : 0;
            }
        }

        public int NumPlayers
        {
            get
            {
                return ResponseFields.ContainsKey("numplayers") ? Convert.ToInt32(ResponseFields["numplayers"]) : 0;
            }
        }
    }
}
