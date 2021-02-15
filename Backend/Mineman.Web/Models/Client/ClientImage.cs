using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Models.Client
{
    public class ClientImage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ModDirectory { get; set; }
        public ImageBuildStatus BuildStatus { get; set; }
        public IEnumerable<int> ServersUsingImage {get; set; }
    }

    public static class ImageExtensions
    {
        public static ClientImage ToClientImage(this Image image, IEnumerable<int> serversUsingImage)
        {
            return new ClientImage
            {
                ID = image.ID,
                Name = image.Name,
                ModDirectory = image.ModDirectory,
                BuildStatus = image.BuildStatus,
                ServersUsingImage = serversUsingImage
            };
        }
    }
}
