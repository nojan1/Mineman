using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    internal class BlockFactory
    {
        public static Block CreateFromData(int blockId, int y, int z, int x, byte biomeId, byte data)
        {
            //TODO: Creation of special blocks
            return new Block(blockId, y, z, x, biomeId, data);
        }
    }
}
