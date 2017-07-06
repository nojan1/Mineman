using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using Mineman.Service.Models.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Repositories
{
    public class RemoteImageRepository : IRemoteImageRepository
    {
        private const string MANIFEST_FILENAME = "manifest.json";

        private readonly RemoteImageOptions _remoteImageOptions;
        private readonly ILogger<RemoteImageRepository> _logger;
        private readonly HttpClient _client;

        private List<RemoteImage> _remoteImages = new List<RemoteImage>();
        private DateTimeOffset? _lastRefresh;
        private bool _refreshRunning = false;

        public RemoteImageRepository(IOptions<RemoteImageOptions> remoteImageOptions,
                                     ILogger<RemoteImageRepository> logger)
        {
            _remoteImageOptions = remoteImageOptions.Value;
            _logger = logger;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_remoteImageOptions.RepositoryPath);
        }

        public ICollection<RemoteImage> Get()
        {
            return _remoteImages;
        }

        public async Task RefreshIfNeeded()
        {
            if (_refreshRunning || !_remoteImageOptions.Enable)
                return;

            if (_lastRefresh  == null || _lastRefresh.Value + _remoteImageOptions.RefreshInterval > DateTimeOffset.Now)
            {
                _refreshRunning = true;

                try
                {
                    var manifest = await _client.GetStringAsync(MANIFEST_FILENAME);
                    _remoteImages = JsonConvert.DeserializeObject<List<RemoteImage>>(manifest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(), ex, "Error when refreshing remote images");
                }
                finally
                {
                    _lastRefresh = DateTimeOffset.Now;
                    _refreshRunning = false;
                }
            }
        }

        public async Task<Stream> GetDownloadStream(string hash)
        {
            var remoteImage = _remoteImages.FirstOrDefault(i => i.SHA256Hash == hash);
            if(remoteImage == null)
            {
                throw new ArgumentException($"No remote image was found with SHA256 hash of: '{hash}'");
            }

            return await _client.GetStreamAsync(remoteImage.FileName);
        }
    }
}
