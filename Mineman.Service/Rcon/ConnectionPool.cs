using CoreRCON;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using Mineman.Service.Helpers;
using Mineman.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Rcon
{
    public class ConnectionPool : IConnectionPool
    {
        private readonly IServerRepository _serverRepository;

        private List<RconConnection> _connections = new List<RconConnection>();
        private IPAddress _rconIP;

        public ConnectionPool(IOptions<Configuration> configuration,
                              IServerRepository serverRepository)
        {
            _rconIP = IPAddress.Parse(configuration.Value.QueryIpAddress);
            _serverRepository = serverRepository;
        }

        public RconConnection GetConnectionForServer(int serverId)
        {
            RconConnection connection = null;

            //lock (_connections)
            //{
                connection = _connections.FirstOrDefault(c => c.ServerId == serverId);
                if(connection == null)
                {
                    var server = _serverRepository.GetWithDockerInfo(serverId).Result;
                    if (!server.IsAlive)
                    {
                        throw new Exception("Server not alive");
                    }

                    var properties = ServerPropertiesSerializer.Deserialize(server.Server.SerializedProperties);

                    connection = new RconConnection(new RCON(_rconIP, (ushort)server.Server.RconPort, properties.Rcon__Password), serverId);
                    _connections.Add(connection);
                //}
            }

            return connection;
        }

        public void DisposeConnectionsOlderThen(TimeSpan span)
        {
            //lock (_connections)
            //{
                var oldConnections = _connections.Where(c => c.LastCommand < DateTimeOffset.Now - span);
                foreach (var oldConnection in oldConnections)
                {
                    oldConnection.Dispose();
                    _connections.Remove(oldConnection);
                }
          //  }
        }
    }
}
