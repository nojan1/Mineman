using System;
using System.Collections.Generic;
using System.Text;
using NBT;
using Mineman.WorldParsing.Blocks;
using System.Linq;

namespace Mineman.WorldParsing.Entities
{
    public class Player : Mob
    {
        public string UUID { get; private set; }
        public InventoryItem[] Inventory { get; set; }

        public Player(string uuid, TagCompound tag) : base("player", tag)
        {
            UUID = uuid;

            Inventory = tag.GetList("Inventory")
                            .Value
                            .Cast<TagCompound>()
                            .Select(t => new InventoryItem(t))
                            .ToArray();
        }

        public override string ToString()
        {
            return "Player";
        }
    }
}
