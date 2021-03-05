using Mineman.WorldParsing.Blocks;
using SixLabors.ImageSharp;

namespace Mineman.WorldParsing.MapTools
{
    public interface ITextureProvider
    {
        Color GetBlocklightColor();
        Color GetColorForBiome(BiomeType biomeType, Color fallbackColor);
        Color GetColorForBlock(Block block, Color fallbackColor);
    }
}