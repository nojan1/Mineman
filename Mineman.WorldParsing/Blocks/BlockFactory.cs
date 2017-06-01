using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    internal class BlockFactory
    {
        public static Block CreateFromId(int blockId, int y, int z, int x, byte biomeId, byte data, byte blockLight, byte skyLight)
        {
            //TODO: Creation of special blocks
            return new Block(blockId, y, z, x, biomeId, data, blockLight, skyLight);
        }
    }
}
