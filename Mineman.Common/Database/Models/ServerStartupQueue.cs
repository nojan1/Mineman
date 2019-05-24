using System;
namespace Mineman.Common.Database.Models
{
    public class ServerStartupQueue
    {
        public int Id { get; set; }
        public int ServerId { get; set; }

        public virtual Server Server { get; set; }
    }
}
