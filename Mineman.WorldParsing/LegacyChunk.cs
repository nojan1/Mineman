using System.Collections.Generic;
using System.Linq;
using Mineman.WorldParsing.Blocks;

namespace Mineman.WorldParsing
{
    public class LegacyChunk : Chunk
    {
        private const int BLOCK_COUNT = 4096;

        private byte[] _blockLight;
        private byte[] _blockIds;
        private byte[] _addBlocks;
        private byte[] _data;
        private byte[] _skylight;
        private byte[] _biomeIds;

        private Column _parentColumn;

        public LegacyChunk(int y, int z, int x, byte[] blockLight, byte[] blockIds, byte[] addBlocks, byte[] data,
            byte[] skylight, byte[] biomeIds, Column parentColumn)
        {
            XWorld = x;
            ZWorld = z;
            YOrder = y;
            _blockIds = blockIds;
            _addBlocks = addBlocks;
            _blockLight = blockLight;
            _skylight = skylight;
            _data = data;
            _biomeIds = biomeIds;
            _parentColumn = parentColumn;
        }

        protected override IEnumerable<Block> GetBlocks()
        {
            if (_blockIds.Length != 0)
            {
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    var blockId = _addBlocks == null || _addBlocks.Length != BLOCK_COUNT
                        ? _blockIds[i]
                        : (_addBlocks[i] << 8) + _blockIds[i];

                    int x = XWorld + (i % 16);
                    int y = (YOrder * 16) + (i / 256);
                    int z = ZWorld + ((i / 16) % 16);

                    byte biomeId = _biomeIds[i % 256];
                    var blockEntity = _parentColumn.BlockEntities.FirstOrDefault(e => e.X == x && e.Y == y && e.Z == z);

                    yield return BlockFactory.CreateFromId(blockId, y, z, x, biomeId, _data[i], _blockLight[i],
                        _skylight[i], blockEntity);
                }
            }
        }
    }
}