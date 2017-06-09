using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using Mineman.Service.Helpers;
using Mineman.Service.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Mod")]
    [Authorize]
    public class ModController : Controller
    {
        private readonly IModRepository _modRepository;
        private readonly Configuration _configuration;
        private readonly IHostingEnvironment _environment;

        public ModController(IModRepository modRepository,
                             IOptions<Configuration> configuration,
                             IHostingEnvironment environment)
        {
            _modRepository = modRepository;
            _configuration = configuration.Value;
            _environment = environment;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(_modRepository.GetMods());
        }

        [HttpPost("")]
        public async Task<IActionResult> Add(ModAddModel inputModel)
        {
            if (inputModel == null ||
                !ModelState.IsValid ||
                inputModel.ModFile.Count != 1)
            {
                return BadRequest();
            }

            var mod = await _modRepository.Add(inputModel);

            return Ok(mod);
        }

        [HttpGet("download/{modId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Download(int modId)
        {
            var mod = await _modRepository.Get(modId);
            if(mod == null)
            {
                return BadRequest();
            }

            var modPath = _environment.BuildPath(_configuration.ModDirectory, mod.Path);

            using (var stream = System.IO.File.OpenRead(modPath))
            {
                return File(stream, "application/octect-stream", mod.Path);
            }
        }
    }
}
