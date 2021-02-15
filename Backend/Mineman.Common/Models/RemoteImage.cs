using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Models
{
    public class RemoteImage
    {
        public string DisplayName { get; set; }
        public string ModDirectory { get; set; }
        public string FileName { get; set; }
        public string SHA256Hash { get; set; }
    }
}
