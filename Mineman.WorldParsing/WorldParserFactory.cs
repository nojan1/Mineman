using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing
{
    public class WorldParserFactory : IWorldParserFactory
    {
        public IWorldParser Create(string worldPath)
        {
            return new WorldParser(worldPath);
        }
    }
}
