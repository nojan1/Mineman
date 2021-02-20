using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mineman.Service.Rcon;
using Mineman.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Mineman.Web.Controllers
{
    [Route("api/server/rcon")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RconController : Controller
    {
        private readonly IConnectionPool _connectionPool;

        public RconController(IConnectionPool connectionPool)
        {
            _connectionPool = connectionPool;
        }

        [HttpPost("{serverId:int}")]
        public async Task<IActionResult> SendCommand(int serverId, [FromBody]RconRequestModel request)
        {
            var connection = _connectionPool.GetConnectionForServer(serverId);

            return Ok(new
            {
                Response = await connection.SendCommandAndGetResponse(request.Command)
            });
        }
    }
}
