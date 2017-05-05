using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using Mineman.Service;
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

        public ServerController(IServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _serverRepository.GetServers());
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
    }
}
