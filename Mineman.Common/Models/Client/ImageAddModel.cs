using Microsoft.AspNetCore.Http;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mineman.Common.Models.Client
{
    public class ImageAddModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public ServerType Type { get; set; }
        public string ModDir { get; set; }
        [Required]
        public ICollection<IFormFile> ImageContents { get; set; }
    }
}
