using Cyotek.Data.Nbt;
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

            switch (id.ToLower())
            {
                case "item":
                    return new Item(id, tag);

                case "sheep":
                case "pig":
                case "chicken":
                case "bat":
                case "blaze":
                case "cave_spider":
                case "cow":
                case "creeper":
                case "donkey":
                case "elder_guardian":
                case "ender_dragon":
                case "enderman":
                case "endermite":
                case "evocation_illager":
                case "ghast":
                case "giant":
                case "guardian":
                case "horse":
                case "husk":
                case "llama":
                case "magma_cube":
                case "mooshroom":
                case "mule":
                case "ocelot":
                case "parrot":
                case "polar_bear":
                case "shulker":
                case "silverfish":
                case "skeleton":
                case "skeleton_horse":
                case "slime":
                case "snowman":
                case "spider":
                case "squid":
                case "stray":
                case "vex":
                case "villager":
                case "villager_golem":
                case "vindidation_illager":
                case "witch":
                case "wither":
                case "wither_skeleton":
                case "wolf":
                case "zombie":
                case "zombie_horse":
                case "zombie_pigman":
                case "zombie_villager":
                    return new Mob(id, tag);

                default:
                    return new Entity(id, tag);
            }
        }
    }
}
