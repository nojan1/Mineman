using NBT;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Entities
{
    internal class EntityFactory
    {
        public static Entity CreateFromTag(TagCompound tag)
        {
            var id = tag.GetStringValue("id");

            switch (id)
            {
                case "Item":
                    return new Item(id, tag);
                case "Sheep":
                case "Pig":
                case "Chicken":
                    return new Animal(id, tag);
                default:
                    return new Entity(id, tag);
            }
        }
    }
}
