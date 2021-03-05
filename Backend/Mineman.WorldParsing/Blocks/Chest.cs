using Cyotek.Data.Nbt;
using Mineman.WorldParsing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    public class InventoryItem
    {
        public InventoryItem(TagCompound tag)
        {
            Id = tag.GetTag("id").ToValueString();
            Count = tag.GetByteValue("Count");
            Damage = tag.GetShortValue("Damage");
            Slot = tag.GetByteValue("Slot");
            Tag = tag.Contains("tag") ? tag.GetCompound("tag") : null;
        }

        public string Id { get; set; }
        public int Count { get; set; }
        public int Slot { get; set; }
        public int Damage { get; set; }
        public TagCompound Tag { get; set; }

        public override string ToString()
        {
            return $"{Count} x {Id}";
        }
    }

    public class Chest : Block
    {
        public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();

        public Chest(int id, int y, int z, int x, byte biomeId, byte data, byte blockLight, byte skyLight, BlockEntity blockEntity) : base(id, y, z, x, biomeId, data, blockLight, skyLight, blockEntity)
        {
            if(blockEntity != null)
            {
                Items = blockEntity.Tag.GetList("Items")?.Value
                .Cast<TagCompound>()
                .Select(t => new InventoryItem(t))
                .ToArray() ?? new InventoryItem[0];
            }
        }

        public override string ToString()
        {
            return $"Chest ({Items.Count} items)";
        }
    }
}
