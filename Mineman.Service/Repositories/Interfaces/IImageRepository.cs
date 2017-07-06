using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Client;
using Mineman.Common.Models;

namespace Mineman.Service.Repositories
{
    public interface IImageRepository
    {
        Task<Image> Add(ImageAddModel imageAddModel);
        ICollection<Image> GetImages();
        Image Get(int imageId);
        Task Delete(int imageId);
        IDictionary<int, Server[]> GetImageUsage();
        Task<Image> AddRemote(RemoteImage remoteImage);
    }
}