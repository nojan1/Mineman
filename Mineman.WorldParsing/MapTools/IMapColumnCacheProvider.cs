using Mineman.WorldParsing.MapTools.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.MapTools
{
    public interface IMapColumnCacheProvider
    {
        RegionMapCache GetRegionMapCache(int x, int z);
        void SaveRegionMapCache(int x, int z, RegionMapCache regionMapCache);
    }
}
