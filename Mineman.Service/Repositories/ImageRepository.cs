using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly DatabaseContext _context;
        private readonly Configuration _configuration;
        private readonly IHostingEnvironment _environment;

        public ImageRepository(DatabaseContext context,
                               IOptions<Configuration> configuration,
                               IHostingEnvironment environment)
        {
            _context = context;
            _configuration = configuration.Value;
            _environment = environment;
        }

        public ICollection<Image> Get()
        {
            return _context.Images.ToList();
        }

        public async Task Add(ImageAddModel imageAddModel)
        {
            string zipName = $"{Guid.NewGuid().ToString("N")}.zip";
            string imageContentZipPath = Path.Combine(_environment.ContentRootPath, _configuration.ImageZipFileDirectory, zipName);
            
            using(var file = File.Create(imageContentZipPath))
            {
                var stream = imageAddModel.ImageContents.First().OpenReadStream();
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(file);
            }

            var server = new Image
            {
                ImageContentZipPath = zipName,
                Type = imageAddModel.Type,
                SupportsMods = !string.IsNullOrEmpty(imageAddModel.ModDir),
                ModDirectory = imageAddModel.ModDir ?? "",
                CreatedInDocker = false
            };

            await _context.Images.AddAsync(server);
            await _context.SaveChangesAsync();
        }
    }
}
