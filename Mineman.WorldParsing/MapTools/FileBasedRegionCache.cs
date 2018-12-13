using System;
using System.Collections.Generic;
using System.Text;
using Mineman.WorldParsing.MapTools.Models;
using System.IO;
using Newtonsoft.Json;
using ProtoBuf;

namespace Mineman.WorldParsing.MapTools
{
    public class FileBasedRegionCache : IMapColumnCacheProvider
    {
        private readonly string cacheFolder;

        public FileBasedRegionCache(string baseFolder)
        {
            cacheFolder = Path.Combine(baseFolder, "blockcache");

            if (!Directory.Exists(cacheFolder))
                Directory.CreateDirectory(cacheFolder);
        }

        public RegionMapCache GetRegionMapCache(int x, int z)
        {
            var path = buildFileName(x, z);
            if (!File.Exists(path))
                return null;

            using (var file = File.OpenRead(path))
            {
                try
                {
                    var regionMapCache = Serializer.Deserialize<RegionMapCache>(file);
                    return regionMapCache;
                }
                catch (ProtoException)
                {
                    //Most likely the cache type format has changed or the file is corrupt. 
                    //Ignore so that the file will be overwritten with correct data
                    return null;
                }
            }
        }

        public void SaveRegionMapCache(int x, int z, RegionMapCache regionMapCache)
        {
            using (var file = File.Create(buildFileName(x, z)))
            {
                Serializer.Serialize(file, regionMapCache);
            }
        }

        private string buildFileName(int x, int z)
        {
            return Path.Combine(cacheFolder, $"{x}-{z}.bin");
        }

    }
}
