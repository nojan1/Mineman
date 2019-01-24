using CoreRCON;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Rcon
{
    public class RconConnection : RCON
    {
        public RconConnection(IPAddress host, ushort port, string password, int serverId)
            : base(host, port, password)
        {
            ServerId = serverId;
        }

        public DateTimeOffset LastCommand { get; private set; }
        public int ServerId { get; private set; }

        public async Task<string> SendCommandAndGetResponse(string command)
        {
            LastCommand = DateTimeOffset.Now;
            return await SendCommandAsync(command);
        }
    }
}
