using System;

namespace Mineman.Service.Rcon
{
    public interface IConnectionPool
    {
        void DisposeConnectionsOlderThen(TimeSpan span);
        RconConnection GetConnectionForServer(int serverId);
    }
}