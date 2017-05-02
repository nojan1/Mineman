using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Common.Models
{
    public class Configuration
    {
        public string DockerHost { get; set; }
        public string WorldDirectory { get; set; }
        public string ServerPropertiesDirectory { get; set; }
        public string ModDirectory { get; set; }
    }
}
