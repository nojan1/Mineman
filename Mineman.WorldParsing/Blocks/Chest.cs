using Mineman.WorldParsing.Entities;
using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    public class ChestItem
    {
        public string Id { get; set; }
        public int Count { get; set; }
        public int Slot { get; set; }
        public int Damage { get; set; }
        public TagCompound Tag { get; set; }

        public override string ToString()
        {
            return $"{Count}x {Id}";
        }
    }

    public class Chest : Block
    {
        public ICollection<ChestItem> Items { get; set; }

        public Chest(int id, int y, int z, int x, byte biomeId, byte data, byte blockLight, byte skyLight, BlockEntity blockEntity) : base(id, y, z, x, biomeId, data, blockLight, skyLight, blockEntity)
        {
            Items = blockEntity.Tag.GetList("Items").Value
                .Cast<TagCompound>()
                .Select(t => new ChestItem
                    {
                        Id = t.GetStringValue("id"),
                        Count = t.GetByteValue("Count"),
                        Damage = t.GetShortValue("Damage"),
                        Slot = t.GetByteValue("Slot"),
                        Tag = t.Contains("tag") ? t.GetCompound("tag") : null
                    })
                .ToArray();
        }

        public override string ToString()
        {
            return $"Chest ({Items.Count} items)";
        }
    }
}
