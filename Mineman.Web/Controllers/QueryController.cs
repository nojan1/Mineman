using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mineman.Service.MinecraftQuery;
using Mineman.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/server/query")]
    [Authorize]
    public class QueryController : Controller
    {
        private readonly IServerRepository _serverRepository;
        private readonly IMinecraftServerQuery _minecraftServerQuery;

        public QueryController(IServerRepository serverRepository,
                               IMinecraftServerQuery minecraftServerQuery)
        {
            _serverRepository = serverRepository;
            _minecraftServerQuery = minecraftServerQuery;
        }

        [HttpGet("{serverId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int serverId)
        {
            var server = await _serverRepository.Get(serverId);
            if(server == null)
            {
                return BadRequest();
            }

            var queryInfo = await _minecraftServerQuery.GetInfo(server);

            return Ok(queryInfo);
        }
    }
}
