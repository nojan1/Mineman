using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Managers
{
    public interface IImageManager
    {
        Task CreateImage(Image image);
        Task RemoveUnsuedImages();
        Task InvalidateMissingImages();
        Task DeleteImage(Image image);
    }
}