using ImageSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mineman.WorldParsing.MapTools.Models
{
    public class BlockColor
    {
        public (int, int) Coordinates { get; set; }
        public uint Argb { get; set; }
    }

    public class ColumnRenderCacheItem
    {
        public (int, int) Coordinates { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public List<BlockColor> BlockColors { get; set; }
    }

    public class RegionMapCache
    {
        public List<ColumnRenderCacheItem> ColumnsCache { get; set; } = new List<ColumnRenderCacheItem>();

        public ColumnRenderCacheItem GetColumn(int columnX, int columnZ)
        {
            return ColumnsCache.FirstOrDefault(x => x.Coordinates.Equals((columnX, columnZ)));
        }

        public bool ColumnCacheIsStale(int columnX, int columnZ, DateTimeOffset targetTimestamp)
        {
            var column = GetColumn(columnX, columnZ);
            return column == null || column.Timestamp < targetTimestamp;
        }

        public void UpdateTimestamp(int columnX, int columnZ, DateTimeOffset newTimestamp)
        {
            var column = GetColumn(columnX, columnZ);
            if(column == null)
            {
                column = new ColumnRenderCacheItem
                {
                    Coordinates = (columnX, columnZ),
                    BlockColors = new List<BlockColor>()
                };

                ColumnsCache.Add(column);
            }

            column.Timestamp = newTimestamp;
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
