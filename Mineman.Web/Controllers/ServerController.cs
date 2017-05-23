using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using Mineman.Service;
using Mineman.Service.Helpers;
using Mineman.Service.Managers;
using Mineman.Service.MinecraftQuery;
using Mineman.Service.Repositories;
using Mineman.Web.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Server")]
    [Authorize]
    public class ServerController : Controller
    {
        private readonly IServerRepository _serverRepository;
        private readonly IServerManager _serverManager;

        public ServerController(IServerRepository serverRepository,
                                IServerManager serverManager)
        {
            _serverRepository = serverRepository;
            _serverManager = serverManager;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var servers = (await _serverRepository.GetServers())
                                   .Select(x => x.Server.ToClientServer(x.IsAlive));

            return Ok(servers);
        }

        [HttpGet("{serverId:int}")]
        public async Task<IActionResult> Get(int serverId)
        {
            var serverWithDockerInfo = await _serverRepository.GetWithDockerInfo(serverId);
            var properties = ServerPropertiesSerializer.GetUserChangableProperties();

            return Ok(new
            {
                server = serverWithDockerInfo.Server,
                isAlive = serverWithDockerInfo.IsAlive,
                properties = properties
            });
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody]ServerAddModel inputModel)
        {
            if (inputModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var server = await _serverRepository.Add(inputModel);

            return Ok(server);
        }

        [HttpPost("start/{serverId:int}")]
        public async Task<IActionResult> Start(int serverId)
        {
            var server = await _serverRepository.Get(serverId);
            var result = await _serverManager.Start(server);

            return Ok(new { success = result });
        }

        [HttpPost("stop/{serverId:int}")]
        public async Task<IActionResult> Stop(int serverId)
        {
            var server = await _serverRepository.Get(serverId);
            var result = await _serverManager.Stop(server);

            return Ok(new { success = result });
        }

        [HttpPost("restart/{serverId:int}")]
        public async Task<IActionResult> Restart(int serverId, bool forceRecreate)
        {
            var server = await _serverRepository.Get(serverId);
            server.NeedsRecreate = forceRecreate;

            if (await _serverManager.Stop(server))
            {
                return Ok(new { success = await _serverManager.Start(server) });
            }
            else
            {
                return Ok(new { success = false });
            }
        }

        [HttpPost("{serverId:int}")]
        public async Task<IActionResult> UpdateConfiguration(int serverId, [FromBody]ServerConfigurationModel configurationModel)
        {
            if(configurationModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var server = await _serverRepository.UpdateConfiguration(serverId, configurationModel);

            return Ok(server);
        }
    }
}
