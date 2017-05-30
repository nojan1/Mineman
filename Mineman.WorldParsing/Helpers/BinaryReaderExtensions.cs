using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mineman.WorldParsing.Helpers
{
    public static class BinaryReaderExtensions
    {
        public static int ReadInt32BE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytesRequired(sizeof(Int32));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader reader, int byteCount)
        {
            var result = reader.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }
    }
}
