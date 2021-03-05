using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImageController : Controller
    {
        private readonly IImageRepository _imageRepository;
        private readonly IImageManager _imageManager;
        private readonly ILogger<ImageController> _logger;
        private readonly IRemoteImageRepository _remoteImageRepository;

        public ImageController(IImageRepository imageRepository,
                               IImageManager imageManager,
                               ILogger<ImageController> logger,
                               IRemoteImageRepository remoteImageRepository)
        {
            _imageRepository = imageRepository;
            _imageManager = imageManager;
            _logger = logger;
            _remoteImageRepository = remoteImageRepository;
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
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Adding new image to database. Name: '{inputModel.DisplayName}'");

            var image = await _imageRepository.Add(inputModel);

            _logger.LogInformation($"Image was added. Name: '{inputModel.DisplayName}'");

            return Ok(image);
        }

        [HttpDelete("{imageId:int}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            var image = _imageRepository.Get(imageId);

            try
            {
                _logger.LogInformation($"About to delete image from database. ImageID: '{imageId}' Name: '{image.Name}'");
                await _imageRepository.Delete(imageId);
            }
            catch (ImageInUseException)
            {
                _logger.LogError($"Unable to delete image, still in use by server. ImageID: '{imageId}' Name: '{image.Name}'");
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            if (!string.IsNullOrEmpty(image.DockerId))
            {
                _logger.LogInformation($"About to delete image in docker. ImageID: '{imageId}' Name: '{image.Name}' DockerId: '{image.DockerId}'");
                await _imageManager.DeleteImage(image);
            }

            return NoContent();
        }

        [HttpGet("remote/")]
        public IActionResult GetRemoteImages()
        {
            return Ok(_remoteImageRepository.Get());
        }

        [HttpPost("remote/{hash}")]
        public async Task<IActionResult> AddRemoteImage(string hash)
        {
            var remoteImage = _remoteImageRepository.Get()
                                .FirstOrDefault(x => x.SHA256Hash == hash);

            if(remoteImage == null)
            {
                return BadRequest();
            }

            var image = await _imageRepository.AddRemote(remoteImage);

            return Ok(image);
        }
    }
}
