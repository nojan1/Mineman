using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Models;
using System.IO;

namespace Mineman.Service.Repositories
{
    public interface IRemoteImageRepository
    {
        ICollection<RemoteImage> Get();
        Task RefreshIfNeeded();
        Task<Stream> GetDownloadStream(string hash);
    }
}