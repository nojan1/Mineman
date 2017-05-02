using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Database.Models
{
    public class Server
    {
        public int ID { get; set; }
        public string ContainerID { get; set; }
        public string Description { get; set; }
        public int MemoryAllocationMB { get; set; }
        public Image Image { get; set; }
        public World World { get; set; }
        public ICollection<Mod> Mods { get; set; }
        public int MainPort { get; set; }
        public int QueryPort { get; set; }
        public int RconPort { get; set; }
        public string SerializedProperties { get; set; }
        public bool ShouldBeRunning { get; set; }
    }
}
