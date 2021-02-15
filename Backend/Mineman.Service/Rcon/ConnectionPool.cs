using CoreRCON;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using Mineman.Common.Models.Configuration;
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
        private readonly IServiceScopeFactory _serviceFactory;

        private List<RconConnection> _connections = new List<RconConnection>();
        private IPAddress _rconIP;

        public ConnectionPool(IOptions<ServerCommunicationOptions> configuration,
                              IServiceScopeFactory serviceFactory)
        {
            _rconIP = IPAddress.Parse(configuration.Value.QueryIpAddress);
            _serviceFactory = serviceFactory;
        }

        public RconConnection GetConnectionForServer(int serverId)
        {
            RconConnection connection = null;

            //lock (_connections)
            //{
            //connection = _connections.FirstOrDefault(c => c.ServerId == serverId);
            if (connection == null)    
            {
                using (var scope = _serviceFactory.CreateScope())
                {
                    var server = scope.ServiceProvider.GetService<IServerRepository>().GetWithDockerInfo(serverId).Result;
                    if (!server.IsAlive)
                    {
                        throw new Exception("Server not alive");
                    }

                    var properties = ServerPropertiesSerializer.Deserialize(server.Server.SerializedProperties);

                    connection = new RconConnection(_rconIP, (ushort)server.Server.RconPort, properties.Rcon__Password, serverId);
                    //_connections.Add(connection);
                }
            }
            //}

            return connection;
        }

        public void DisposeConnectionsOlderThen(TimeSpan span)
        {
            // //lock (_connections)
            // //{
            // var oldConnections = _connections.Where(c => c.LastCommand < DateTimeOffset.Now - span).ToList();
            // foreach (var oldConnection in oldConnections)
            // {
            //     oldConnection.Dispose();
            //     _connections.Remove(oldConnection);
            // }
            //  }
        }
    }
}
