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

    public class WorldInfoModel
    {
        public IEnumerable<PlayerInfoModel> Players { get; set; }
    }
}
