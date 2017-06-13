using Mineman.WorldParsing.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mineman.WorldParsing
{
    public class Region
    {
        private const int SECTOR_BYTES = 4096;
        private const int SECTOR_INTS = SECTOR_BYTES / 4;

        public int X { get; private set; }
        public int Z { get; private set; }

        private string _reqionFilePath;

        public IEnumerable<Column> Columns
        {
            get
            {
                return GetColumns();
            }
        }

        public Region(string regionFilePath)
        {
            var fileInfo = new FileInfo(regionFilePath);

            var filenameParts = fileInfo.Name.Split('.');
            if (filenameParts.Length != 4 || filenameParts[0] != "r")
            {
                throw new ArgumentException("Specified region file has invalid name");
            }

            X = Convert.ToInt32(filenameParts[1]);
            Z = Convert.ToInt32(filenameParts[2]);

            _reqionFilePath = regionFilePath;
        }

        private IEnumerable<Column> GetColumns()
        {
            using (var stream = File.OpenRead(_reqionFilePath))
            {
                var offsets = new List<int>();
                var timestamps = new List<int>();

                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < SECTOR_INTS; i++)
                    {
                        int offset = reader.ReadInt32BE();
                        offsets.Add(offset);
                    }

                    for (int i = 0; i < SECTOR_INTS; i++)
                    {
                        int timestamp = reader.ReadInt32BE();
                        timestamps.Add(timestamp);
                    }

                    for (int i = 0; i < offsets.Count; i++)
                    {
                        if (offsets[i] == 0)
                            continue;

                        int sectorNumber = offsets[i] >> 8;
                        int numSectors = offsets[i] & 0xFF;

                        if (numSectors == 0)
                            continue;

                        stream.Seek(sectorNumber * SECTOR_BYTES, SeekOrigin.Begin);

                        int length = reader.ReadInt32BE();

                        ChunkFormat format = (ChunkFormat)reader.ReadByte();
                        var data = reader.ReadBytes(length - 1);
                        
                        yield return new Column(format,
                                                new MemoryStream(data),
                                                UnixDateTimeHelpers.ToDateTimeOffset(timestamps[i]));
                    }

                }
            }
        }
    }
}
