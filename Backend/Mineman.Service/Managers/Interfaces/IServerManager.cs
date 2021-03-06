﻿using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Managers
{
    public interface IServerManager
    {
        Task<ServerStartResult> Start(Server server);
        Task<bool> Stop(Server server);
        Task<bool> DestroyContainer(Server server);
        Task RemoveUnusedContainers();
    }
}