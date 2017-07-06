using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Service.Models.Configuration
{
    public class RemoteImageOptions
    {
        public bool Enable { get; set; }
        public string RepositoryPath { get; set; }
        public TimeSpan RefreshInterval { get; set; }
    }
}
