using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Database.Models
{
    public enum ServerType
    {
        Vanilla = 1,
        Forge = 2,
    }

    public class Image
    {
        public int ID { get; set; }
        public string Tag { get; set; }
        public string DockerId { get; set; }
        public ServerType Type { get; set; }
        public bool SupportsMods { get; set; }
        public string ModDirectory { get; set; }
        public ImageBuildStatus BuildStatus { get; set; }
        public string ImageContentZipPath { get; set; }
    }
}
