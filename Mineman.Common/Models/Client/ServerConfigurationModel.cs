using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mineman.Common.Models.Client
{
    public class ServerConfigurationModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
        [Required]
        [Range(1024, 56535)]
        public int ServerPort { get; set; }
        [Required]
        [Range(256, 10000)]
        public int MemoryAllocationMB { get; set; }
        [Required]
        public int WorldID { get; set; }
        [Required]
        public int ImageID { get; set; }
        public ICollection<int> ModIDs { get; set; }
        [Required]
        public ServerProperties Properties { get; set; }
    }
}
