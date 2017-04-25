using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Models
{
    public enum ServerType {
        Vanilla = 1,
        Forge = 2,
    }

    public class Server
    {
        public string ImageName { get; set; }
        public string Description { get; set; }
        public ServerType Type { get; set; }
        public World World { get; set; }
        public ICollection<Mod> Mods { get; set; }
        public int MainPort { get; set; }
        public int QueryPort { get; set; }
        public int RconPort { get; set; }
    }
}
