using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Blocks
{
    public class Sign : Block
    {
        public Sign(int id, int y, int z, int x, byte biomeId, byte data) : base(id, y, z, x, biomeId, data)
        {
        }

        public string[] TextLines { get; private set; }

       
    }
}
