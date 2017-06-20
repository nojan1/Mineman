using Mineman.WorldParsing.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mineman.WorldParsing.Tests
{
    public class RegionParsingTests
    {
        [Theory]
        [InlineData(RegionType.Overworld, 9)]
        [InlineData(RegionType.Nether, 0)]
        [InlineData(RegionType.EndWorld, 0)]
        public void CorrectNumberOfRegionsAreDetected(RegionType type, int count)
        {
            var parser = WorldParserHelper.CreateForTestWorld();

            Assert.Equal(count, parser.GetRegions(type).Count());
        }

        [Fact]
        public void ParsedRegionsHaveTheCorrectCoordinates()
        {
            var parser = WorldParserHelper.CreateForTestWorld();
            var regions = parser.GetRegions(RegionType.Overworld).ToList();

            Assert.Contains(regions, v => v.X == -2 && v.Z == 0);
            Assert.Contains(regions, v => v.X == -2 && v.Z == 1);
            Assert.Contains(regions, v => v.X == -1 && v.Z == -1);
            Assert.Contains(regions, v => v.X == -1 && v.Z == 0);
            Assert.Contains(regions, v => v.X == -1 && v.Z == 1);
            Assert.Contains(regions, v => v.X == -1 && v.Z == 2);
            Assert.Contains(regions, v => v.X == 0 && v.Z == -1);
            Assert.Contains(regions, v => v.X == 0 && v.Z == 0);
            Assert.Contains(regions, v => v.X == 0 && v.Z == 1);

        }
    }
}
