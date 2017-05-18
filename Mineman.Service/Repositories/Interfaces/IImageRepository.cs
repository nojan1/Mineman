using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Client;

namespace Mineman.Service.Repositories
{
    public interface IImageRepository
    {
        Task Add(ImageAddModel imageAddModel);
        ICollection<Image> GetImages();
    }
}