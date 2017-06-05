﻿using Mineman.WorldParsing.Blocks;
using NBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;

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

        public int Modified { get; private set; }

        private NbtDocument _nbtDoc;

        public Column(ChunkFormat format, MemoryStream chunkDataStream, int timestamp)
        {
            Modified = timestamp;

            var stream = GetDecompressedStream(format, chunkDataStream); //TODO: Need to dispose of this manually?
            _nbtDoc = NbtDocument.LoadDocument(stream);

            var level = _nbtDoc.Query<TagCompound>("Level");
            XWorld = level.GetIntValue("xPos") * 16;
            ZWorld = level.GetIntValue("zPos") * 16;
            TerrainPopulated = level.GetByteValue("TerrainPopulated") == 1;
        }

        public IEnumerable<Chunk> Chunks
        {
            get
            {
                return GetChunks();
            }
        }

        public override string ToString()
        {
            return $"Chunk[{XWorld},{ZWorld}]";
        }

        private IEnumerable<Chunk> GetChunks()
        {
            var level = _nbtDoc.Query<TagCompound>("Level");
            var biomeIds = level.GetByteArrayValue("Biomes");

            foreach (TagCompound section in level.GetList("Sections").Value)
            {
                int y = section.GetByteValue("Y");
                var blockIds = section.GetByteArrayValue("Blocks");
                var blockLight = To4BitValues(section.GetByteArrayValue("BlockLight"));
                var skyLight = To4BitValues(section.GetByteArrayValue("SkyLight"));
                var blockData = To4BitValues(section.GetByteArrayValue("Data"));
                var addData = To4BitValues(section.GetByteArrayValue("AddBlocks"));

                yield return new Chunk(y, ZWorld, XWorld, blockLight, blockIds, addData, blockData, skyLight, biomeIds);
            }
        }

        private Stream GetDecompressedStream(ChunkFormat format, MemoryStream chunkDataStream)
        {
            var outputStream = new MemoryStream();

            switch (format)
            {
                case ChunkFormat.ZLIB_DEFLATE:
                    if (chunkDataStream.ReadByte() != 0x78 || chunkDataStream.ReadByte() != 0x9C)//zlib header
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
            for(int i = 0; i < data.Length; i++)
            {
                dataOut.Add((byte)(data[i] >> 4));
                dataOut.Add((byte)(data[i] & 0b00001111));
            }

            return dataOut.ToArray();
        }
    }
}