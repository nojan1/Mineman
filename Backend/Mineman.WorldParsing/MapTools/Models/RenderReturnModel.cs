using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.MapTools.Models
{
    public class RenderReturnModel
    {
        public Image<Rgba32> Bitmap { get; set; }
        public int OffsetX { get; set; }
        public int OffsetZ { get; set; }
        public Dictionary<string, int> UnknownRenderEntites { get; set; }
    }
}
