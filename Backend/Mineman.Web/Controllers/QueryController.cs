using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            try
            {
                var server = await _serverRepository.Get(serverId);
                if (server == null)
                {
                    return BadRequest();
                }

                var queryInfo = await _minecraftServerQuery.GetInfo(server);

                return Ok(new
                {
                    Result = "success",
                    Data = queryInfo
                });
            }
            catch (TimeoutException)
            {
                return StatusCode(500, new
                {
                    Result = "timeout"
                });
            }
        }
    }
}
