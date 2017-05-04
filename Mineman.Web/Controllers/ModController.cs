using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Mod")]
    public class ModController : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody]ModAddModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
