using Mineman.WorldParsing.Blocks;
using NBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;
using Mineman.WorldParsing.Entities;
using System.Linq;

namespace Mineman.WorldParsing
{
    public enum ChunkFormat
    {
        UNCOMPRESSED = 0,
        GZIP = 1,
        ZLIB_DEFLATE = 2
    }

    public class Column
    {
        public bool TerrainPopulated { get; private set; }
        public int XWorld { get; private set; }
        public int ZWorld { get; private set; }

        public DateTime Modified { get; private set; }

        private readonly NbtDocument _nbtDoc;

        public Column(ChunkFormat format, MemoryStream chunkDataStream, DateTime timestamp)
        {
            Modified = timestamp;

            var stream = GetDecompressedStream(format, chunkDataStream); //TODO: Need to dispose of this manually?
            _nbtDoc = NbtDocument.LoadDocument(stream);

            var level = _nbtDoc.Query<TagCompound>("Level");
            XWorld = level.GetIntValue("xPos") * 16;
            ZWorld = level.GetIntValue("zPos") * 16;

            var terrainPopulated = level.GetByte("TerrainPopulated");
            if (terrainPopulated != null)
            {
                TerrainPopulated = terrainPopulated.Value == 1;
            }
            else
            {
                TerrainPopulated = level.GetLongValue("LastUpdate") > 0;
            }
        }

        private BlockEntity[] _blockEntities;

        public BlockEntity[] BlockEntities
        {
            get
            {
                if (_blockEntities == null)
                {
                    _blockEntities = GetBlockEntities();
                }

                return _blockEntities;
            }
        }

        public IEnumerable<Chunk> Chunks
        {
            get { return GetChunks(); }
        }

        public IEnumerable<Entity> Entities
        {
            get { return GetEntities(); }
        }

        public override string ToString()
        {
            return $"Chunk[{XWorld},{ZWorld}]";
        }

        private BlockEntity[] GetBlockEntities()
        {
            var level = _nbtDoc.Query<TagCompound>("Level");

            return level
                .GetList("TileEntities") //Was called TileEntities in earlier minecraft version. Official name is now BlockEntities
                .Value
                .Cast<TagCompound>()
                .Select(t => new BlockEntity(t))
                .ToArray();
        }

        private IEnumerable<Entity> GetEntities()
        {
            var level = _nbtDoc.Query<TagCompound>("Level");

            return level.GetList("Entities").Value
                .Cast<TagCompound>()
                .Select(entity => { return EntityFactory.CreateFromTag(entity); });
        }

        private IEnumerable<Chunk> GetChunks()
        {
            var level = _nbtDoc.Query<TagCompound>("Level");

            var biomeIds = level.GetTag("Biomes").Type == TagType.ByteArray
                ? level.GetByteArrayValue("Biomes")
                : level.GetIntArrayValue("Biomes").Select(Convert.ToByte).ToArray();

            foreach (TagCompound section in level.GetList("Sections").Value)
            {
                int y = section.GetByteValue("Y");
                var blockLight = To4BitValues(section.GetByteArrayValue("BlockLight"));

                var blockStates = section.GetTag<TagLongArray>("BlockStates");
                var palette = section.GetList("Palette");

                if (blockStates != null && palette != null)
                {
                    //New format ( >= 1.13 )
                    yield return new NewChunk(y, ZWorld, XWorld, blockStates, palette);
                }
                else
                {
                    //Legacy format ( > 1.9 < 1.13 )

                    var blockIds = section.GetByteArrayValue("Blocks");
                    var skyLight = To4BitValues(section.GetByteArrayValue("SkyLight"));
                    var blockData = To4BitValues(section.GetByteArrayValue("Data"));
                    var addData = To4BitValues(section.GetByteArrayValue("AddBlocks"));

                    yield return new LegacyChunk(y, ZWorld, XWorld, blockLight, blockIds, addData, blockData, skyLight,
                        biomeIds, this);
                }
            }
        }

        private Stream GetDecompressedStream(ChunkFormat format, MemoryStream chunkDataStream)
        {
            var outputStream = new MemoryStream();

            switch (format)
            {
                case ChunkFormat.ZLIB_DEFLATE:
                    if (chunkDataStream.ReadByte() != 0x78 || chunkDataStream.ReadByte() != 0x9C) //zlib header
                        throw new Exception("Incorrect zlib header");

                    using (var deflateStream = new DeflateStream(chunkDataStream, CompressionMode.Decompress, false))
                    {
                        deflateStream.CopyTo(outputStream);
                    }

                    break;
                case ChunkFormat.GZIP:
                    using (var gzipStream = new GZipStream(chunkDataStream, CompressionMode.Decompress, false))
                    {
                        gzipStream.CopyTo(outputStream);
                    }

                    break;
                default:
                    chunkDataStream.CopyTo(outputStream);

                    break;
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }

        private byte[] To4BitValues(byte[] data)
        {
            if (data == null)
                return null;

            var dataOut = new List<byte>(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                dataOut.Add((byte) (data[i] >> 4));
                dataOut.Add((byte) (data[i] & 0b00001111));
            }

            return dataOut.ToArray();
        }
    }
}