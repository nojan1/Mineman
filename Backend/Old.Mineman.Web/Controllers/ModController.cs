using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using Mineman.Common.Models.Configuration;
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
        private readonly PathOptions _pathOptions;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ModController> _logger;

        public ModController(IModRepository modRepository,
                             IOptions<PathOptions> pathOptions,
                             IHostingEnvironment environment,
                             ILogger<ModController> logger)
        {
            _modRepository = modRepository;
            _pathOptions = pathOptions.Value;
            _environment = environment;
            _logger = logger;
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

            _logger.LogInformation($"Adding new mod to database. Name: '{inputModel.DisplayName}'");

            var mod = await _modRepository.Add(inputModel);

            _logger.LogInformation($"Mod was added. Name: '{inputModel.DisplayName}'");

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

            var modPath = _environment.BuildPath(_pathOptions.ModDirectory, mod.Path);

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
                _logger.LogInformation($"About to delete mod. ModId: '{modId}'");
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
