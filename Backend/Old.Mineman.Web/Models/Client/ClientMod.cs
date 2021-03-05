using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Models.Client
{
    public class ClientMod
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<int> ServersUsingMod { get; set; }
    }

    public static class ModExtensions
    {
        public static ClientMod ToClientMod(this Mod mod, IEnumerable<int> serversUsingMod)
        {
            return new ClientMod
            {
               ID = mod.ID,
               DisplayName = mod.DisplayName,
               ServersUsingMod = serversUsingMod
            };
        }
    }
}
