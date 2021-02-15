using Mineman.WorldParsing.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mineman.WorldParsing.Tests
{
    public class CanParseNewFormat
    {
        [Fact]
        public void Test()
        {
            var parser = new WorldParser(@"../../../../Testfiles/world-1.14");

            foreach (var region in parser.GetRegions(RegionType.Overworld))
            {
                foreach (var column in region.Columns)
                {
                    if(!column.TerrainPopulated)
                        continue;
                    
                    foreach (var chunk in column.Chunks)
                    {
                        foreach (var block in chunk.Blocks)
                        {
                            Console.WriteLine($"X: {block.WorldX} Z: {block.WorldZ} Y: {block.WorldY} - {block.Code}");
                        }
                    }
                }
            }
        }
    }
}
