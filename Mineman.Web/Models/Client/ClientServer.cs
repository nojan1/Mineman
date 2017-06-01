using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Models.Client
{
    public class ClientServer
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int MainPort { get; set; }
        public bool IsAlive { get; set; }
        public bool HasMap { get; set; }
    }

    public static class ServerExtensions
    {
        public static ClientServer ToClientServer(this Server server, bool isAlive, bool hasMap)
        {
            return new ClientServer
            {
                ID = server.ID,
                Description = server.Description,
                MainPort = server.MainPort,
                IsAlive = isAlive,
                HasMap = hasMap
            };
        }
    }
}
