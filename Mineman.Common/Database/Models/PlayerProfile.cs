using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Database.Models
{
    public class PlayerProfile
    {
        public int ID { get; set; }
        public DateTimeOffset LastFetched { get; set; }
        public string UUID { get; set; }
        public string Name { get; set; }
        public string SkinURL { get; set; }
    }
}
