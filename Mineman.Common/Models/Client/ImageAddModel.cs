using Microsoft.AspNetCore.Http;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Models.Client
{
    public class ImageAddModel
    {
        public string DisplayName { get; set; }
        public ServerType Type { get; set; }
        public string ModDir { get; set; }
        public ICollection<IFormFile> ImageContents { get; set; }
    }
}
