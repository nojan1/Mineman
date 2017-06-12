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
            var parser = new WorldParser(@"C:\Users\hedlundn\Desktop\world");

            //var region = parser.Regions.First();
            //var column = region.Columns.First();
            //var blockEnties = region.Columns.SelectMany(c => c.BlockEntities).ToList();
            //var entities = region.Columns.SelectMany(c => c.Entities).ToList();
            //var chunk = column.Chunks.First();
            //var blocks = chunk.Blocks.ToList();

            //var blocks = parser.Regions.SelectMany(r => r.Columns)
            //                           .SelectMany(c => c.Chunks)
            //                           .Where(c => c.YOrder > 2 && c.YOrder < 5)
            //                           .SelectMany(c => c.Blocks)
            //                           .ToList();


            var chests = new List<Sign>();
            foreach(var region in parser.Regions)
            {
                foreach(var column in region.Columns)
                {
                    foreach(var chunk in column.Chunks)
                    {
                        chests.AddRange(chunk.Blocks.OfType<Sign>());
                    }
                }
            }


            return;

            var renderer = new MapRenderer2D(parser, new TextureProvider(""));
            //var bitmap = renderer.GenerateBiomeBitmap();
            var bitmap = renderer.GenerateBlockBitmap();
            bitmap.Save(@"C:\Users\hedlundn\Desktop\map.png");
        }
    }
}
