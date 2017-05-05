using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mineman.Common.Models.Client
{
    public class ServerAddModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public int WorldID { get; set; }
        [Required]
        public int ImageID { get; set; }
        public ICollection<int> ModIDs { get; set; }

    }
}
