using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mineman.WorldParsing
{
    public class WorldParser : IWorldParser
    {
        private string _worldPath;

        private List<string> _regionFiles;

        public IEnumerable<Region> Regions
        {
            get
            {
                return GetRegions();
            }
        }

        public WorldParser(string worldPath)
        {
            _worldPath = worldPath;

            var regionDirectory = Path.Combine(worldPath, "region");
            _regionFiles = Directory.EnumerateFiles(regionDirectory, "*.mca")
                                    .ToList();
        }

        private IEnumerable<Region> GetRegions()
        {
            return _regionFiles.Select(s => new Region(s));
        }
    }
}
