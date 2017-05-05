using Docker.DotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Newtonsoft.Json;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mineman.Service.Managers
{
    public class ImageCreationException : Exception
    {
        public string ErrorDetails { get; private set; }

        public ImageCreationException(string message, string errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }
    }

    public class ImageManager : IImageManager
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;
        private readonly Configuration _configuration;
        private readonly IHostingEnvironment _environment;

        public ImageManager(DatabaseContext context,
                            IDockerClient dockerClient,
                            IOptions<Configuration> configuration,
                            IHostingEnvironment environment)
        {
            _context = context;
            _dockerClient = dockerClient;
            _configuration = configuration.Value;
            _environment = environment;
        }

        public async Task CreateImage(Image image)
        {
            if (image.CreatedInDocker)
                return;

            var dockerFilePath = Path.Combine(_environment.ContentRootPath, _configuration.DockerfilePath);
            var workingDir = Path.Combine(_environment.ContentRootPath, _configuration.ImageZipFileDirectory, Guid.NewGuid().ToString("N"));
            var zipPath = Path.Combine(_environment.ContentRootPath, _configuration.ImageZipFileDirectory, image.ImageContentZipPath);

            using (var zipArchive = new ZipArchive(File.OpenRead(zipPath)))
            {
                zipArchive.ExtractToDirectory(workingDir);
            }

            File.Copy(dockerFilePath, Path.Combine(workingDir, "Dockerfile"), true);

            var dockerArchivePath = Path.GetTempFileName();
            using (var stream = File.OpenWrite(dockerArchivePath))
            {
                using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, new WriterOptions(CompressionType.None)))
                {
                    writer.WriteAll(workingDir, "*", SearchOption.AllDirectories);
                }
            }

            var responseStream = await _dockerClient.Miscellaneous.BuildImageFromDockerfileAsync(File.OpenRead(dockerArchivePath), 
                                                    new Docker.DotNet.Models.ImageBuildParameters
                                                    {
                                                        Dockerfile = "Dockerfile",
                                                        Labels = new Dictionary<string, string>()
                                                        {
                                                            { "creator", "mineman" }
                                                        }
                                                    },
                                                    CancellationToken.None);

            var imageId = await WaitForId(responseStream);

            image.CreatedInDocker = true;
            image.DockerId = imageId;

            _context.Update(image);
            await _context.SaveChangesAsync();
        }

        private async Task<string> WaitForId(Stream responseStream)
        {
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = (await reader.ReadLineAsync()).Trim();
                    if (line.Trim().StartsWith("{"))
                    {
                        if (line.Contains("error"))
                        {
                            dynamic obj = JsonConvert.DeserializeObject(line);

                            throw new ImageCreationException(obj.error, obj.errorDetail);
                        }
                    }
                    else
                    {
                        return line;
                    }
                }

                throw new ImageCreationException("No ID recived from build", null);
            }
        }
    }
}
