using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using Mineman.Service;
using Mineman.Service.Managers;
using Mineman.Service.MinecraftQuery;
using Mineman.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Server")]
    public class ServerController : Controller
    {
        private readonly IServerRepository _serverRepository;
        private readonly IServerManager _serverManager;
        private readonly IMinecraftServerQuery _minecraftServerQuery;

        public ServerController(IServerRepository serverRepository,
                                IServerManager serverManager,
                                IMinecraftServerQuery minecraftServerQuery)
        {
            _serverRepository = serverRepository;
            _serverManager = serverManager;
            _minecraftServerQuery = minecraftServerQuery;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _serverRepository.GetServers());
        }

        [HttpGet("query/{serverId:int}")]
        public async Task<IActionResult> GetQuery(int serverId)
        {
            var server = await _serverRepository.Get(serverId);
            var queryInfo = await _minecraftServerQuery.GetInfo(server);

            return Ok(queryInfo);
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody]ServerAddModel inputModel)
        {
            if (inputModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            await _serverRepository.Add(inputModel);

            return Ok();
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

        [HttpPost("recreate/{serverId:int}")]
        public async Task<IActionResult> Recreate(int serverId)
        {
            var server = await _serverRepository.Get(serverId);

            if (await _serverManager.DestroyContainer(server))
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
