using ImageSharp;

namespace Mineman.WorldParsing.MapTools
{
    public interface IMapRenderer2D
    {
        Image<Rgba32> GenerateBiomeBitmap();
        Image<Rgba32> GenerateBlockBitmap();
    }
}