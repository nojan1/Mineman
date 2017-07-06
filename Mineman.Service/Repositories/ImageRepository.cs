using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using Mineman.Common.Models.Configuration;
using Mineman.Service.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Repositories
{
    public class ImageInUseException : Exception { }

    public class ImageRepository : IImageRepository
    {
        private readonly DatabaseContext _context;
        private readonly PathOptions _pathOptions;
        private readonly IHostingEnvironment _environment;

        public ImageRepository(DatabaseContext context,
                               IOptions<PathOptions> pathOptions,
                               IHostingEnvironment environment)
        {
            _context = context;
            _pathOptions = pathOptions.Value;
            _environment = environment;
        }

        public Image Get(int imageId)
        {
            return _context.Images
                            .Include(x => x.BuildStatus)
                            .FirstOrDefault(i => i.ID == imageId);
        } 

        public ICollection<Image> GetImages()
        {
            return _context.Images
                           .Include(x => x.BuildStatus)
                           .ToList();
        }

        public async Task<Image> Add(ImageAddModel imageAddModel)
        {
            string zipName = $"{Guid.NewGuid().ToString("N")}.zip";
            string imageContentZipPath = _environment.BuildPath(_pathOptions.ImageZipFileDirectory, zipName);
            
            using(var file = File.Create(imageContentZipPath))
            {
                var stream = imageAddModel.ImageContents.First().OpenReadStream();
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(file);
            }

            var image = new Image
            {
                Name = imageAddModel.DisplayName,
                ImageContentZipPath = zipName,
                Type = imageAddModel.Type,
                SupportsMods = !string.IsNullOrEmpty(imageAddModel.ModDir),
                ModDirectory = imageAddModel.ModDir ?? "",
                BuildStatus = null
            };

            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();

            return image;
        }

        public async Task<Image> AddRemote(RemoteImage remoteImage)
        {
            var image = new Image
            {
                Name = remoteImage.DisplayName,
                RemoteHash = remoteImage.SHA256Hash,
                Type = ServerType.Vanilla, //TODO: Remove property?
                SupportsMods = !string.IsNullOrEmpty(remoteImage.ModDirectory),
                ModDirectory = remoteImage.ModDirectory ?? "",
                BuildStatus = null
            };

            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();

            return image;
        }

        public async Task Delete(int imageId)
        {
            var imageInUse = await _context.Servers.Include(s => s.Image)
                                                   .Where(s => s.Image != null)
                                                   .Select(s => s.Image.ID)
                                                   .ContainsAsync(imageId);

            if (imageInUse)
            {
                throw new ImageInUseException();
            }

            var image = await _context.Images.FindAsync(imageId);
            _context.Images.Remove(image);

            await _context.SaveChangesAsync();
        }

        public IDictionary<int, Server[]> GetImageUsage()
        {
            var servers = _context.Servers.Include(s => s.Image).ToList();
            var images = _context.Images.ToList();

            return images.ToDictionary(i => i.ID,
                i => servers.Where(s => s.Image?.ID == i.ID).ToArray());

        }
    }
}
