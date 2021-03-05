using Mineman.WorldParsing.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mineman.WorldParsing.Tests
{
    public class PlayerParsingTests
    {
        [Fact]
        public void PlayersAreParsedCorrectly()
        {
            var parser = WorldParserHelper.CreateForTestWorld();
            var players = parser.Players.ToList();

            Assert.Equal(1, players.Count);

            Assert.Equal(16.76, players.First().Health, 2);
            Assert.Equal("1b785d21-567b-4ef2-be7e-df040c8a1db3", players.First().UUID);
            Assert.Equal("player", players.First().Id);
            Assert.Equal(510.227, players.First().Z, 3);
            Assert.Equal(-310.665, players.First().X, 3);
            Assert.Equal(58, players.First().Y);
        }

        [Fact]
        public void PlayerInventoryIsParsedCorrectly()
        {
            var parser = WorldParserHelper.CreateForTestWorld();
            var inventory = parser.Players.First().Inventory;

            Action<string, int, int, int> CheckInventoryContains = (id, count, damage, slot) =>
            {
                Assert.True(inventory.SingleOrDefault(i => i.Id == id &&
                                                           i.Slot == slot &&
                                                           i.Count == count &&
                                                           i.Damage == damage) != null,
                            $"ID: {id} not found in inventory");
            };

            Assert.Equal(25, inventory.Length);

            CheckInventoryContains("minecraft:iron_sword", 1, 241, 0);
            CheckInventoryContains("minecraft:iron_axe", 1, 243, 1);
            CheckInventoryContains("minecraft:iron_shovel", 1, 57, 2);
            CheckInventoryContains("minecraft:iron_pickaxe", 1, 65, 3);
            CheckInventoryContains("minecraft:bread", 12, 0, 7);
            CheckInventoryContains("minecraft:torch", 29, 0, 8);
            CheckInventoryContains("minecraft:cobblestone", 64, 0, 14);
            CheckInventoryContains("minecraft:wheat", 2, 0, 15);
            CheckInventoryContains("minecraft:cobblestone", 64, 0, 16);
            CheckInventoryContains("minecraft:stone", 50, 3, 17);
            CheckInventoryContains("minecraft:iron_sword", 1, 0, 18);
            CheckInventoryContains("minecraft:iron_shovel", 1, 0, 19);
            CheckInventoryContains("minecraft:stick", 32, 0, 24);
            CheckInventoryContains("minecraft:cobblestone", 64, 0, 25);
            CheckInventoryContains("minecraft:gravel", 50, 0, 26);
            CheckInventoryContains("minecraft:iron_pickaxe", 1, 0, 27);
            CheckInventoryContains("minecraft:iron_axe", 1, 0, 28);
            CheckInventoryContains("minecraft:bow", 1, 117, 29);
            CheckInventoryContains("minecraft:cobblestone", 42, 0, 32);
            CheckInventoryContains("minecraft:stone", 12, 1, 33);
            CheckInventoryContains("minecraft:dirt", 20, 0, 34);
            CheckInventoryContains("minecraft:dirt", 64, 0, 35);
            CheckInventoryContains("minecraft:iron_boots", 1, 181, 100);
            CheckInventoryContains("minecraft:iron_leggings", 1, 181, 101);
            CheckInventoryContains("minecraft:iron_chestplate", 1, 181, 102);
        }
    }
}
