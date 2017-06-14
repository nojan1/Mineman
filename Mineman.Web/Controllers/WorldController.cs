using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Client;
using Mineman.Service.Repositories;
using Mineman.Web.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return Ok(_worldRepository.Get());
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

    }
}
