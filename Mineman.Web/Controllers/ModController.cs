using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using Mineman.Service.Helpers;
using Mineman.Service.Repositories;
using Mineman.Web.Models.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public IActionResult Get()
        {
            var mods = _modRepository.GetMods();
            var modUsage = _modRepository.GetModUsage();

            return Ok(mods.Select(m => m.ToClientMod(modUsage.ContainsKey(m.ID) ? modUsage[m.ID].Select(s => s.ID).ToArray() : new int[0])));
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

        [HttpGet("download/{modId:int}")]
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

        [HttpDelete("{modId:int}")]
        public async Task<IActionResult> Delete(int modId)
        {
            try
            {
                await _modRepository.Delete(modId);
            }
            catch (ModInUseException)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            return NoContent();
        }
    }
}
