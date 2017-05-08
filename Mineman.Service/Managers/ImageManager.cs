using Docker.DotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Mineman.Service.Models;
using Newtonsoft.Json;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private readonly Configuration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ImageManager> _logger;

        public ImageManager(DatabaseContext context,
                            IDockerClient dockerClient,
                            IOptions<Configuration> configuration,
                            IHostingEnvironment environment,
                            ILogger<ImageManager> logger)
        {
            _context = context;
            _dockerClient = dockerClient;
            _configuration = configuration.Value;
            _environment = environment;
            _logger = logger;
        }

        public async Task CreateImage(Image image)
        {
            if (image.BuildStatus != null)
                return;

            var dockerFilePath = Path.Combine(_environment.ContentRootPath, _configuration.DockerfilePath);
            var workingDir = Path.Combine(_environment.ContentRootPath, _configuration.ImageZipFileDirectory, Guid.NewGuid().ToString("N"));
            var zipPath = Path.Combine(_environment.ContentRootPath, _configuration.ImageZipFileDirectory, image.ImageContentZipPath);

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

                logBuilder = new StringBuilder();
                imageId = await WaitForId(responseStream, logBuilder);
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"Error building image! ImageID: '{image.ID}'");
                buildSucceeded = false;
            }
            finally
            {
                //Cleanup
                try { File.Delete(zipPath); } catch { }
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
    }
}
