using Mineman.WorldParsing.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mineman.WorldParsing.Tests
{
    public class LevelDatParsingTests
    {
        [Fact]
        public void SpawnPointIsParsedCorrectly()
        {
            var parser = WorldParserHelper.CreateForTestWorld();

            Assert.Equal(-124, parser.Level.SpawnX);
            Assert.Equal(64, parser.Level.SpawnY);
            Assert.Equal(140, parser.Level.SpawnZ);
        }
    }
}
