using Mineman.WorldParsing;
using Mineman.WorldParsing.Blocks;
using Mineman.WorldParsing.MapTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mineman.Service.Tests
{
    public class temp
    {
        [Fact]
        public void lsdgj()
        {
            var parser = new WorldParser(@"C:\Users\hedlundn\Desktop\world-oldbackup");

            //var region = parser.GetRegions(RegionType.Overworld).First();
            //var column = region.Columns.First();
            //var blockEnties = region.Columns.SelectMany(c => c.BlockEntities).ToList();
            //var chunk = column.Chunks.First();
            //var blocks = chunk.Blocks.ToList();

            //var blocks = parser.GetRegions(RegionType.Overworld).SelectMany(r => r.Columns)
            //                           .SelectMany(c => c.Entities)
            //                           .ToList();

            var renderer = new MapRenderer2D(parser, new TextureProvider(""));
            //var bitmap = renderer.GenerateBiomeBitmap();

            renderer.GenerateBlockBitmap(RegionType.Overworld).Save(@"C:\Users\hedlundn\Desktop\map-overworld.png");
        }
    }
}
