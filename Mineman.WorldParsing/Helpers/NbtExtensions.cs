using NBT;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Helpers
{
    public static class UnixDateTimeHelpers
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                      DateTimeKind.Utc);
        public static DateTimeOffset ToDateTimeOffset(long milliseconds)
        {
            return Epoch.AddMilliseconds(milliseconds);
        }

        public static DateTimeOffset ToDateTimeOffset(int seconds)
        {
            return Epoch.AddSeconds(seconds);
        }
    }

    public static class NbtExtensions
    {
        public static DateTimeOffset ToDateTimeOffset(this TagLong tag)
        {
            return UnixDateTimeHelpers.ToDateTimeOffset(tag.Value);
        }
    }
}
