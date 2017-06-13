using Mineman.WorldParsing.Entities;
using NBT;
using System.Collections.Generic;

namespace Mineman.WorldParsing
{
    public interface IWorldParser
    {
        TagCompound Level { get; }
        IEnumerable<Player> Players { get; }
        IEnumerable<Region> GetRegions(RegionType regionType);
    }
}