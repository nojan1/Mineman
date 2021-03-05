using Microsoft.AspNetCore.SignalR;
using Mineman.Service;
using Mineman.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Helpers
{
    public class InfoHubClient : IInfoClient
    {
        private readonly IHubContext<InfoHub, IInfoClient> _hubContext;

        public InfoHubClient(IHubContext<InfoHub, IInfoClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task ImageBuildComplete(int serverId) =>
            _hubContext.Clients.All.ImageBuildComplete(serverId);

        public Task ServerStarted(int serverId) =>
            _hubContext.Clients.All.ServerStarted(serverId);

        public Task ServerStopped(int serverId) =>
            _hubContext.Clients.All.ServerStopped(serverId);
    }
}
