
using Cyotek.Data.Nbt;

namespace Mineman.WorldParsing.Entities
{
    public class Item : Entity
    {
        public string ItemId { get; set; }
        public int Count { get; set; }
        public int Damage { get; set; }

        public Item(string id, TagCompound tag) : base(id, tag)
        {
            var item = tag.GetCompound("Item");

            ItemId = item.GetTag("id").GetValue().ToString();
            Count = item.GetByteValue("Count");
            Damage = item.GetShortValue("Damage");
        }

        public override string ToString()
        {
            return $"Item: {ItemId}";
        }
    }
}