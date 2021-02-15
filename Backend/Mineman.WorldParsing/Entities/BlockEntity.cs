using Cyotek.Data.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Entities
{
    public class BlockEntity
    {
        public string Id { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
        public TagCompound Tag { get; private set; }

        public BlockEntity(TagCompound tag)
        {
            Tag = tag;

            Id = tag.GetStringValue("id");
            X = tag.GetIntValue("x");
            Y = tag.GetIntValue("y");
            Z = tag.GetIntValue("z");
        }

        public BlockEntity(TagCompound tag, string id, int x, int y, int z)
        {
            Tag = tag;

            Id = id;
            X = x;
            Y = y;
            Z = z;
        }
        
        public override string ToString()
        {
            return $"Entity: {Id}";
        }
    }
}
