using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mineman.WorldParsing
{
    public enum RegionType
    {
        Overworld,
        Nether,
        EndWorld
    }

    public class WorldParser : IWorldParser
    {
        private string _worldPath;

        private List<string> _regionFiles;

        public WorldParser(string worldPath)
        {
            _worldPath = worldPath;
        }

        public IEnumerable<Region> GetRegions(RegionType regionType)
        {
            var regionPaths = new Dictionary<RegionType, string>
            {
                { RegionType.Overworld, "region" },
                { RegionType.EndWorld, "DIM1/region" },
                { RegionType.Nether, "DIM-1/region" },
            };

            var regionDirectory = Path.Combine(_worldPath, regionPaths[regionType]);
            if (!Directory.Exists(regionDirectory))
            {
                return new Region[0];
            }

            return Directory.EnumerateFiles(regionDirectory, "*.mca")
                    .Select(regionFile => new Region(regionFile));
        }
    }
}
