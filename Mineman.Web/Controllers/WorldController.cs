using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using Mineman.Service.Repositories;
using Mineman.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/World")]
    public class WorldController : Controller
    {
        private readonly IWorldRepository _worldRepository;

        public WorldController(IWorldRepository worldRepository)
        {
            _worldRepository = worldRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(_worldRepository.Get());
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody]WorldAddModel inputModel)
        {
            if (inputModel == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if(inputModel.WorldFile == null || inputModel.WorldFile.Count < 1)
            {
                await _worldRepository.AddEmpty(inputModel.DisplayName);
            }
            else
            {
                await _worldRepository.AddFromZip(inputModel.DisplayName,
                                                  FileUploadHelper.ZipFromFormFile(inputModel.WorldFile.First()));
            }

            return Ok();
        }
    }
}
