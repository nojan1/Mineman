using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mineman.Common.Models.Client
{
    public class ModAddModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public ICollection<IFormFile> ModFile { get; set; }
    }
}
