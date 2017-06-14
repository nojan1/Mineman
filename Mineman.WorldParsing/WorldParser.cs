using Mineman.WorldParsing.Entities;
using NBT;
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
        public LevelInfo Level { get; private set; }
        public IEnumerable<Player> Players { get; private set; }

        private readonly string _worldPath;

        public WorldParser(string worldPath)
        {
            _worldPath = worldPath;

            LoadLevelDat();
            LoadPlayerDats();
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

        private void LoadLevelDat()
        {
            var levelDatPath = Path.Combine(_worldPath, "level.dat");
            if (File.Exists(levelDatPath))
            {
                var levelDocument = NbtDocument.LoadDocument(levelDatPath);
                var levelTag = levelDocument.DocumentRoot.GetCompound("Data");

                Level = new LevelInfo(levelTag);
            }
        }

        private void LoadPlayerDats()
        {
            var playerDir = Path.Combine(_worldPath, "playerdata");
            if (Directory.Exists(playerDir))
            {
                Players = Directory.EnumerateFiles(playerDir, "*.dat")
                            .Select(path =>
                            {
                                var playerDoc = NbtDocument.LoadDocument(path);
                                var uuid = Path.GetFileNameWithoutExtension(path);

                                return new Player(uuid, playerDoc.DocumentRoot);
                            });
            }
            else
            {
                Players = new List<Player>();
            }
        }
    }
}
