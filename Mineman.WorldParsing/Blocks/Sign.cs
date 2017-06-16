using Mineman.WorldParsing.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    public class Sign : Block
    {
        public Sign(int id, int y, int z, int x, byte biomeId, byte data, byte blockLight, byte skyLight, BlockEntity blockEntity) : base(id, y, z, x, biomeId, data, blockLight, skyLight, blockEntity)
        {
            if (blockEntity != null)
            {
                TextLines = new string[]
                {
                    blockEntity.Tag.GetStringValue("Text1"),
                    blockEntity.Tag.GetStringValue("Text2"),
                    blockEntity.Tag.GetStringValue("Text3"),
                    blockEntity.Tag.GetStringValue("Text4")
                };
            }
        }

        public string[] TextLines { get; private set; } = new string[] { "", "", "", "" };
    }
}
