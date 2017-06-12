using ImageSharp;

namespace Mineman.WorldParsing.MapTools
{
    public interface IMapRenderer2D
    {
        Image<Rgba32> GenerateBiomeBitmap(RegionType regionType);
        Image<Rgba32> GenerateBlockBitmap(RegionType regionType);
    }
}