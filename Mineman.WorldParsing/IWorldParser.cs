using System.Collections.Generic;

namespace Mineman.WorldParsing
{
    public interface IWorldParser
    {
        IEnumerable<Region> Regions { get; }
    }
}