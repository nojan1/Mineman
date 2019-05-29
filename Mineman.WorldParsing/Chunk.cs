using Mineman.WorldParsing.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mineman.WorldParsing
{
    public abstract class Chunk
    {
        public int XWorld { get; protected set; }
        public int ZWorld { get; protected set; }
        public int YOrder { get; protected set; }

        public IEnumerable<Block> Blocks => GetBlocks();

        protected abstract IEnumerable<Block> GetBlocks();
    }
}