using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Service.Models
{
    public class InventoryItemModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class PlayerInfoModel
    {
        public double Health { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string UUID { get; set; }
        public IEnumerable<InventoryItemModel> Inventory { get; set; }
    }

    public class ChestInfoModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public IEnumerable<InventoryItemModel> Items { get; set; }
    }

    public class SignInfoModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public string Text { get; set; }
    }

    public class WorldInfoModel
    {
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public int SpawnZ { get; set; }
        public IEnumerable<PlayerInfoModel> Players { get; set; }
        public IEnumerable<ChestInfoModel> Chests { get; set; }
        public IEnumerable<SignInfoModel> Signs { get; set; }
    }
}
