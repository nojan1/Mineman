using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service
{
    public interface IInfoClient
    {
        Task ServerStarted(int serverId);
        Task ServerStopped(int serverId);
        Task ImageBuildComplete(int serverId);
    }
}
