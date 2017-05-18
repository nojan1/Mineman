using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using Mineman.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Image")]
    [Authorize]
    public class ImageController : Controller
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(_imageRepository.GetImages());
        }

        [HttpPost("")]
        public async Task<IActionResult> Add(ImageAddModel inputModel)
        {
            if (inputModel == null ||
                !ModelState.IsValid ||
                inputModel.ImageContents.Count != 1)
            {
                return BadRequest();
            }

            await _imageRepository.Add(inputModel);

            return Ok();
        }
    }
}
