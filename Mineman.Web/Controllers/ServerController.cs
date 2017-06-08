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
        private readonly IWorldRepository _worldRepository;

        public ServerController(IServerRepository serverRepository,
                                IServerManager serverManager,
                                IWorldRepository worldRepository)
        {
            _serverRepository = serverRepository;
            _serverManager = serverManager;
            _worldRepository = worldRepository;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var servers = (await _serverRepository.GetServers())
                                   .Select(x =>
                                   {
                                       var mapPaths = _worldRepository.GetMapImages(x.Server.ID).Result;
                                       return x.Server.ToClientServer(x.IsAlive, !string.IsNullOrEmpty(mapPaths.MapPath));
                                   })
                                   .OrderBy(c => c.IsAlive)
                                   .ToList();

            return Ok(servers);
        }

        [HttpGet("{serverId:int}")]
        public async Task<IActionResult> Get(int serverId)
        {
            var serverWithDockerInfo = await _serverRepository.GetWithDockerInfo(serverId);
            var properties = ServerPropertiesSerializer.GetUserChangableProperties();
            var mapPaths = serverWithDockerInfo.Server.World != null ? await _worldRepository.GetMapImages(serverWithDockerInfo.Server.World.ID)
                                                                     : new Service.Models.MapImagePaths { MapPath = null };

            return Ok(new
            {
                hasMapImage = !string.IsNullOrEmpty(mapPaths.MapPath),
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

            return Ok(new { result = (int)result });
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

        [HttpPost("imagechange/{serverId:int}")]
        public async Task<IActionResult> ChangeImage(int serverId, int newImageId)
        {
            var server = await _serverRepository.GetWithDockerInfo(serverId);

            if (server.IsAlive)
            {
                if (!await _serverManager.Stop(server.Server))
                    throw new Exception($"Need to stop server before switching image but the server didn't stop. ServerId: {serverId}");
            }

            return Ok(_serverRepository.ChangeImage(serverId, newImageId));
        }

        [HttpDelete("destroy/{serverId:int}")]
        public async Task<IActionResult> Destroy(int serverId)
        { 
            var server = await _serverRepository.Get(serverId);
            var result = await _serverManager.DestroyContainer(server);

            return Ok(new { success = result });
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

        [HttpGet("map/{serverId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Map(int serverId, bool thumb)
        {
            var server = await _serverRepository.Get(serverId);
            var paths = await _worldRepository.GetMapImages(server.World.ID);
            var pathToUse = thumb ? paths.MapThumbPath : paths.MapPath;

            if (string.IsNullOrEmpty(pathToUse))
            {
                return BadRequest();
            }
            else
            {
                return File(System.IO.File.OpenRead(pathToUse), "image/png");
            }
        }
    }
}
