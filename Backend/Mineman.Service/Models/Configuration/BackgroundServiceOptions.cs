using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Service.Models.Configuration
{
    public class BackgroundServiceOptions
    {
        public TimeSpan WorkingLoopSleepInterval { get; set; }
        public bool EnableBackgroundWorldProcessing { get; set; }
        public TimeSpan WorldProcessingInterval { get; set; }
    }
}
