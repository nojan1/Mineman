using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;

namespace Mineman.WorldParsing.MapTools.Models
{
    [ProtoContract]
    public class BlockColor
    {
        [ProtoMember(1)]
        public (int, int) Coordinates { get; set; }
        [ProtoMember(2)]
        public uint Argb { get; set; }
    }

    [ProtoContract]
    public class ColumnRenderCacheItem
    {
        [ProtoMember(1)]
        public (int, int) Coordinates { get; set; }
        [ProtoMember(3)]
        public long TimestampRaw { get; set; }
        [ProtoMember(2)]
        public List<BlockColor> BlockColors { get; set; }

        public DateTime Timestamp
        {
            get
            {
                return new DateTime(TimestampRaw);
            }
        }
    }

    [ProtoContract]
    public class RegionMapCache
    {
        [ProtoMember(1)]
        public List<ColumnRenderCacheItem> ColumnsCache { get; set; } = new List<ColumnRenderCacheItem>();

        public ColumnRenderCacheItem GetColumn(int columnX, int columnZ)
        {
            return ColumnsCache.FirstOrDefault(x => x.Coordinates.Equals((columnX, columnZ)));
        }

        public bool ColumnCacheIsStale(int columnX, int columnZ, DateTime targetTimestamp)
        {
            var column = GetColumn(columnX, columnZ);
            return column == null || column.Timestamp < targetTimestamp;
        }

        public void UpdateTimestamp(int columnX, int columnZ, DateTime newTimestamp)
        {
            var column = GetColumn(columnX, columnZ);
            if (column == null)
            {
                column = new ColumnRenderCacheItem
                {
                    Coordinates = (columnX, columnZ),
                    BlockColors = new List<BlockColor>()
                };

                ColumnsCache.Add(column);
            }

            column.TimestampRaw = newTimestamp.Ticks;
        }

        public void SetBlockColor(int columnX, int columnZ, int blockX, int blockZ, Rgba32 color)
        {
            var column = GetColumn(columnX, columnZ);
            if (column == null)
                throw new Exception("Error no such column");

            var blockColor = column.BlockColors.FirstOrDefault(x => x.Coordinates.Equals((blockX, blockZ)));
            if (blockColor == null)
            {
                blockColor = new BlockColor { Coordinates = (blockX, blockZ) };
                column.BlockColors.Add(blockColor);
            }

            blockColor.Argb = color.PackedValue;
        }
    }
}
