using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Database.Models
{
    public class ImageBuildStatus
    {
        public int ID { get; set; }
        public bool BuildSucceeded { get; set; }
        public string Log { get; set; }
    }
}
