using Cyotek.Data.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing
{
    public class LevelInfo
    {
        public int SpawnX { get; private set; }
        public int SpawnY { get; private set; }
        public int SpawnZ { get; private set; }
        public Tag Version { get; private set; }
        public TagCompound Tag { get; private set; }

        public LevelInfo(TagCompound tag)
        {
            Tag = tag;

            SpawnX = tag.GetIntValue("SpawnX");
            SpawnY = tag.GetIntValue("SpawnY");
            SpawnZ = tag.GetIntValue("SpawnZ");
            Version = tag.GetTag("Version");
        }
    }
}
