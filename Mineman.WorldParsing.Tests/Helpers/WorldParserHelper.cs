using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Tests.Helpers
{
    public static class WorldParserHelper
    {
        public static WorldParser CreateForTestWorld()
        {
            return new WorldParser(@"../../../../Testfiles/world");
        }
    }
}
