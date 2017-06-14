using ImageSharp;
using Mineman.WorldParsing.MapTools.Models;

namespace Mineman.WorldParsing.MapTools
{
    public interface IMapRenderer2D
    {
        RenderReturnModel GenerateBiomeBitmap(RegionType regionType);
        RenderReturnModel GenerateBlockBitmap(RegionType regionType);
    }
}