using Mineman.WorldParsing.Blocks;
using SixLabors.ImageSharp.PixelFormats;

namespace Mineman.WorldParsing.MapTools
{
    public interface ITextureProvider
    {
        Rgba32 GetBlocklightColor();
        Rgba32 GetColorForBiome(BiomeType biomeType, Rgba32 fallbackColor);
        Rgba32 GetColorForBlock(Block block, Rgba32 fallbackColor);
    }
}