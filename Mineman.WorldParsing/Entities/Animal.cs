using NBT;

namespace Mineman.WorldParsing.Entities
{
    public class Animal : Entity
    {
        public Animal(string id, TagCompound tag) : base(id, tag)
        {
        }

        public override string ToString()
        {
            return $"Animal: {Id}";
        }
    }
}