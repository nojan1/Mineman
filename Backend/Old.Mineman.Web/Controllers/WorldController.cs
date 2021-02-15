using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Client;
using Mineman.Service.Repositories;
using Mineman.Web.Helpers;
using Mineman.Web.Models.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/World")]
    [Authorize]
    public class WorldController : Controller
    {
        private readonly IWorldRepository _worldRepository;

        public WorldController(IWorldRepository worldRepository)
        {
            _worldRepository = worldRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var worlds = _worldRepository.Get();
            var worldUsage = _worldRepository.GetWorldUsage();

            return Ok(worlds.Select(w => w.ToClientWorld(worldUsage.ContainsKey(w.ID) ? worldUsage[w.ID].Select(s => s.ID).ToArray() : new int[0])));
        }

        [HttpPost("")]
        public async Task<IActionResult> Add(WorldAddModel inputModel)
        {
            if (inputModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            World world;

            if(inputModel.WorldFile == null || inputModel.WorldFile.Count < 1)
            {
                world = await _worldRepository.AddEmpty(inputModel.DisplayName);
            }
            else
            {
                world = await _worldRepository.AddFromZip(inputModel.DisplayName,
                                                  FileUploadHelper.ZipFromFormFile(inputModel.WorldFile.First()));
            }

            return Ok(world);
        }

        [HttpDelete("{worldId:int}")]
        public async Task<IActionResult> Delete(int worldId)
        {
            try
            {
                await _worldRepository.Delete(worldId);
                return Ok();
            }
            catch (WorldInUseException)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }
        }
    }
}
