using CoreRCON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Rcon
{
    public class RconConnection : IDisposable
    {
        private readonly RCON _rcon;

        public RconConnection(RCON rcon, int serverId)
        {
            _rcon = rcon;
            ServerId = serverId;
        }

        public DateTimeOffset LastCommand { get; private set; }
        public int ServerId { get; private set; }

        public async Task<string> SendCommandAndGetResponse(string command)
        {
            LastCommand = DateTimeOffset.Now;

            return await _rcon.SendCommandAsync(command);
        }

        public void Dispose()
        {
            _rcon.Dispose();
        }
    }
}
