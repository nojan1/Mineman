using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Mineman.Common.Models.Configuration;
using Mineman.Service.Helpers;
using Mineman.Service.Models;
using Mineman.Service.Models.Configuration;
using Newtonsoft.Json;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mineman.Service.Managers
{
    public class ImageCreationException : Exception
    { 
        public ImageCreationException(string message) : base(message) {}
    }

    public class ImageManager : IImageManager
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;
        private readonly PathOptions _pathOptions;
        private readonly DockerOptions _dockerOptions;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ImageManager> _logger;

        public ImageManager(DatabaseContext context,
                            IDockerClient dockerClient,
                            IOptions<PathOptions> pathOptions,
                            IOptions<DockerOptions> dockerOptions,
                            IHostingEnvironment environment,
                            ILogger<ImageManager> logger)
        {
            _context = context;
            _dockerClient = dockerClient;
            _pathOptions = pathOptions.Value;
            _dockerOptions = dockerOptions.Value;
            _environment = environment;
            _logger = logger;
        }

        public async Task RemoveUnsuedImages()
        {
            var imageIdsFromDb = _context.Images.Select(i => i.DockerId)
                                                .Where(id => id != null)
                                                .ToList();

            var imagesFromDocker = await DockerQueryHelper.GetImages(_dockerClient);

            foreach(var image in imagesFromDocker.Where(i => !imageIdsFromDb.Any(i2 =>
            {
                var cleanedId = i.ID.Substring(i.ID.IndexOf(':') + 1);
                return cleanedId.StartsWith(i2);
            })))
            {
                _logger.LogInformation($"Found image in docker that was not found in database, removing. ImageID: {image.ID}");

                if(await DeleteDockerImage(image.ID) == false)
                {
                    _logger.LogWarning($"Image may not have been removed correctly");
                }
            }
        }

        public async Task CreateImage(Image image)
        {
            _logger.LogInformation($"About to create docker image for mineman image. ImageID: {image.ID}");

            var dockerFilePath = _environment.BuildPath(_dockerOptions.DockerfilePath);
            var workingDir = _environment.BuildPath(_pathOptions.ImageZipFileDirectory, Guid.NewGuid().ToString("N"));
            var zipPath = _environment.BuildPath(_pathOptions.ImageZipFileDirectory, image.ImageContentZipPath);

            string imageId = null;
            var logBuilder = new StringBuilder();
            bool buildSucceeded = true;
            var dockerArchivePath = Path.GetTempFileName();

            try
            {
                using (var zipArchive = new ZipArchive(File.OpenRead(zipPath)))
                {
                    zipArchive.ExtractToDirectory(workingDir);
                }

                File.Copy(dockerFilePath, Path.Combine(workingDir, "Dockerfile"), true);

                _logger.LogInformation($"Image creation folder preparation complete. ImageID: {image.ID}");

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
                                                            //TODO: Fix so that tags can be used on built images...
                                                            //Tags = new List<string>
                                                            //{
                                                            //    image.Name.Replace(' ', '_')
                                                            //},
                                                            Labels = new Dictionary<string, string>()
                                                            {
                                                            { "creator", "mineman" }
                                                            }
                                                        },
                                                        CancellationToken.None);

                _logger.LogInformation($"Parsing build log and waiting for docker image id. ImageID: {image.ID}");

                logBuilder = new StringBuilder();
                imageId = await WaitForId(responseStream, logBuilder);

                _logger.LogInformation($"Image created in docker. ImageID: {image.ID}, DockerID: {imageId}");
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"Error building image! ImageID: '{image.ID}'");
                buildSucceeded = false;
            }
            finally
            {
                //Cleanup
                try { File.Delete(dockerArchivePath); } catch { }
                try { Directory.Delete(workingDir, true);  } catch { }
            }

            image.BuildStatus = new ImageBuildStatus
            {
                BuildSucceeded = buildSucceeded,
                Log = logBuilder.ToString()
            };
            image.DockerId = imageId;

            _context.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateMissingImages()
        {
            var imagesIdsFromDocker = (await DockerQueryHelper.GetImages(_dockerClient))
                                           .Select(i => i.ID.Substring(i.ID.IndexOf(':') + 1));

            var imagesFromDb = _context.Images.Include(i => i.BuildStatus)
                                              .Where(i => i.DockerId != null)
                                              .ToList();
            
            foreach(var image in imagesFromDb.Where(i => !imagesIdsFromDocker.Any(i2 => i2.StartsWith(i.DockerId))))
            {
                image.DockerId = null;

                if(image.BuildStatus != null)
                {
                    _context.Remove(image.BuildStatus);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<string> WaitForId(Stream responseStream, StringBuilder buildLog)
        {
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = (await reader.ReadLineAsync()).Trim();

                    var streamObject = new DockerImageCreationStreamObject(line);
                    if(streamObject.Type == DockerImageCreationStreamObjectType.Info)
                    {
                        buildLog.AppendLine(streamObject.Info);

                        var match = Regex.Match(streamObject.Info, "Successfully built ([a-f0-9]*)");
                        if(match.Success && match.Groups.Count == 2)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    else
                    {
                        throw new ImageCreationException(streamObject.Info);
                    }
                }

                throw new ImageCreationException("No ID recived from build");
            }
        }

        public async Task DeleteImage(Image image)
        {
            if (image.BuildStatus?.BuildSucceeded == true)
            {
                await DeleteDockerImage(image.DockerId);
            }

            var zipPath = _environment.BuildPath(_pathOptions.ImageZipFileDirectory, image.ImageContentZipPath);
            File.Delete(zipPath);
        }

        private async Task<bool> DeleteDockerImage(string imageName)
        {
            var result = await _dockerClient.Images.DeleteImageAsync(imageName, new ImageDeleteParameters
            {

            });

            if (!result.Any(x => x.Keys.Contains("Deleted")))
            {
                _logger.LogWarning($"Image may not have been removed correctly");
            }

            return result.Any(x => x.Keys.Contains("Deleted"));
        }
    }
}
