using Cyotek.Data.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Helpers
{
    public static class UnixDateTimeHelpers
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                      DateTimeKind.Utc);
        public static DateTime ToDateTime(long milliseconds)
        {
            return Epoch.AddMilliseconds(milliseconds);
        }

        public static DateTime ToDateTime(int seconds)
        {
            return Epoch.AddSeconds(seconds);
        }
    }

    public static class NbtExtensions
    {
        public static DateTime ToDateTime(this TagLong tag)
        {
            return UnixDateTimeHelpers.ToDateTime(tag.Value);
        }
    }
}
