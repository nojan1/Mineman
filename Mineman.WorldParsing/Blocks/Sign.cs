using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    public class Sign : Block
    {
        public Sign(int id, int y, int z, int x, byte biomeId, byte data, byte blockLight, byte skyLight) : base(id, y, z, x, biomeId, data, blockLight, skyLight)
        {
        }

        public string[] TextLines { get; private set; }

       
    }
}
