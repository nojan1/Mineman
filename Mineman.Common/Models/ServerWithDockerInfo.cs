using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Models
{
    public class ServerWithDockerInfo
    {
        public Server Server { get; set; }
        public bool IsAlive { get; set; }
    }
}
