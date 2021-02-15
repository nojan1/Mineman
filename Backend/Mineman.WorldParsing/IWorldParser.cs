using Mineman.WorldParsing.Entities;
using System.Collections.Generic;

namespace Mineman.WorldParsing
{
    public interface IWorldParser
    {
        LevelInfo Level { get; }
        IEnumerable<Player> Players { get; }
        IEnumerable<Region> GetRegions(RegionType regionType);
    }
}