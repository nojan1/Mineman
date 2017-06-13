using System;
using System.Collections.Generic;
using System.Text;
using NBT;

namespace Mineman.WorldParsing.Entities
{
    public class Mob : Entity
    {
        public double Health { get; private set; }

        public Mob(string id, TagCompound tag) : base(id, tag)
        {
            Health = Convert.ToDouble(tag.GetTag("Health").GetValue());
        }

        public override string ToString()
        {
            return $"Mob: {Id}";
        }
    }
}
