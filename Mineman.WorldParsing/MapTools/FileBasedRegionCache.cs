using System;
using System.Collections.Generic;
using System.Text;
using Mineman.WorldParsing.MapTools.Models;
using System.IO;
using Newtonsoft.Json;

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

            var contents = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<RegionMapCache>(contents);
        }

        public void SaveRegionMapCache(int x, int z, RegionMapCache regionMapCache)
        {
            var contents = JsonConvert.SerializeObject(regionMapCache);
            File.WriteAllText(buildFileName(x, z), contents);
        }

        private string buildFileName(int x, int z)
        {
            return Path.Combine(cacheFolder, $"{x}-{z}.json");
        }

    }
}
