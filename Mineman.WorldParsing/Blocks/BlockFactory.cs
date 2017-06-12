using Mineman.WorldParsing.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    internal class BlockFactory
    {
        public static Block CreateFromId(int blockId, int y, int z, int x, byte biomeId, byte data, byte blockLight, byte skyLight, BlockEntity blockEntity)
        {
            switch (blockId)
            {
                case 54:
                    return new Chest(blockId, y, z, x, biomeId, data, blockLight, skyLight, blockEntity);
                case 63:
                case 68:
                case 323:
                    return new Sign(blockId, y, z, x, biomeId, data, blockLight, skyLight, blockEntity);
                default:
                    return new Block(blockId, y, z, x, biomeId, data, blockLight, skyLight, blockEntity);
            }
        }
    }
}
