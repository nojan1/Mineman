using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mineman.Common.Models.Client;
using Mineman.Service.Managers;
using Mineman.Service.Repositories;
using Mineman.Web.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/Image")]
    [Authorize]
    public class ImageController : Controller
    {
        private readonly IImageRepository _imageRepository;
        private readonly IImageManager _imageManager;

        public ImageController(IImageRepository imageRepository,
                               IImageManager imageManager)
        {
            _imageRepository = imageRepository;
            _imageManager = imageManager;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var images = _imageRepository.GetImages();
            var imageUsage = _imageRepository.GetImageUsage();

            return Ok(images.Select(i => i.ToClientImage(imageUsage.ContainsKey(i.ID) ? imageUsage[i.ID].Select(s => s.ID).ToArray() : new int[0])));
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

            var image = await _imageRepository.Add(inputModel);

            return Ok(image);
        }

        [HttpDelete("{imageId:int}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            var image = _imageRepository.Get(imageId);

            try
            {
                await _imageRepository.Delete(imageId);
            }
            catch (ImageInUseException)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            await _imageManager.DeleteImage(image);

            return NoContent();
        }
    }
}
