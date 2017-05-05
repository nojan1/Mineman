using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Models.Client
{
    public class WorldAddModel
    {
        public string DisplayName { get; set; }
        public ICollection<IFormFile> WorldFile { get; set; }
    }
}
