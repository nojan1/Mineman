using System.Collections.Generic;
using Mineman.WorldParsing.Blocks;
using NBT;

namespace Mineman.WorldParsing
{
    public class NewChunk : Chunk
    {
        private readonly TagLongArray _blockStates;
        private readonly TagList _palette;

        public NewChunk(int y, int z, int x, TagLongArray blockStates, TagList palette)
        {
            XWorld = x;
            ZWorld = z;
            YOrder = y;

            _blockStates = blockStates;
            _palette = palette;
        }

        protected override IEnumerable<Block> GetBlocks()
        {
//            foreach (TagCompound block in _palette.Value)
//            {
//                var name = block.GetStringValue("Name");
//                var properties = block.GetCompound("Properties");
//
//                if (properties != null)
//                {
//                    foreach (var x in properties.Value)
//                    {
//                        
//                    }
//                }
//            }

            bool carryOver = false;
            long currentValue = 0;
            for (int i = 0; i < _blockStates.Value.Length; i++)
            {
                long correctHalf = i % 2 == 0
                    ? _blockStates.Value[i] & 0xFFFFFFFF
                    : (_blockStates.Value[i] >> 32) & 0xFFFFFFFF;

                currentValue = carryOver ? currentValue += correctHalf : correctHalf;
                
                if (correctHalf % 64 == 0)
                {
                    carryOver = false;
                    
                        
                }
                else
                {
                    carryOver = true;
                }
            }

            yield return new Block(0, 0,0, 0, 0, 0, 0, 0,  null);
        }
    }
}