using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Models.Client
{
    public class ClientWorld
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<int> ServersUsingWorld { get; set; }
    }

    public static class WorldExtensions
    {
        public static ClientWorld ToClientWorld(this World world, IEnumerable<int> serversUsingWorld)
        {
            return new ClientWorld
            {
                ID = world.ID,
                DisplayName = world.DisplayName,
                ServersUsingWorld = serversUsingWorld
            };
        }
    }
}
