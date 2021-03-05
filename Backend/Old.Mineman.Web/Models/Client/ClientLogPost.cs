using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Models.Client
{
    public class ClientLogPost
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Content { get; set; }
    }
}
