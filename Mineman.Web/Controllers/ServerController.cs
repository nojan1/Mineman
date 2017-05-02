using Microsoft.AspNetCore.Mvc;
using Mineman.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Server")]
    public class ServerController : Controller
    {
        private readonly ServerRepository _serverRepository;

        public ServerController(ServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _serverRepository.GetServers());
        }
    }
}
