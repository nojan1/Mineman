using ImageSharp;
using Mineman.WorldParsing.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Mineman.WorldParsing.MapTools
{
    public class MapRenderer2D : IMapRenderer2D
    {
        private readonly IWorldParser _parser;
        private readonly ITextureProvider _textureProvider;

        public MapRenderer2D(IWorldParser parser,
                             ITextureProvider textureProvider)
        {
            _parser = parser;
            _textureProvider = textureProvider;
        }

        public Image<Rgba32> GenerateBlockBitmap()
        {
            var regions = _parser.Regions.ToList();
            
            var minX = regions.Min(r => r.X);
            var maxX = regions.Max(r => r.X);
            var minZ = regions.Min(r => r.Z);
            var maxZ = regions.Max(r => r.Z);

            var imageWidth = (maxX - minX + 1) * 32 * 16;
            var imageHeight = (maxZ - minZ + 1) * 32 * 16;

            var bitmap = new Image<Rgba32>(imageWidth, imageHeight);
            var populated = new List<(int, int)>(16 * 16);

            var biomes = new List<BiomeType>();

            int actualMinX = Int32.MaxValue, actualMaxX = 0, actualMinZ = Int32.MaxValue, actualMaxZ = 0;

            foreach (var region in regions)
            {
                foreach (var column in region.Columns)
                {
                    populated.Clear();
                    foreach (var chunk in column.Chunks.OrderByDescending(c => c.YOrder))
                    {
                        foreach (var block in chunk.Blocks.OrderByDescending(b => b.WorldY))
                        {
                            if (block.BaseId == 0)
                                continue;

                            if (populated.Contains((block.WorldX, block.WorldZ)))
                                continue;

                            if(!biomes.Contains(block.Biome)) biomes.Add(block.Biome);

                            var color = _textureProvider.GetColorForBlock(block, Rgba32.HotPink);
                            
                            populated.Add((block.WorldX, block.WorldZ));

                            var dX = minX * 32 * 16;
                            var dZ = minZ * 32 * 16;

                            var x = block.WorldX - dX;
                            var z = block.WorldZ - dZ;

                            actualMinX = Math.Min(actualMinX, x);
                            actualMaxX = Math.Max(actualMaxX, x);
                            actualMinZ = Math.Min(actualMinZ, z);
                            actualMaxZ = Math.Max(actualMaxZ, z);

                            bitmap[x, z] = color;
                        }

                        if (populated.Count == (16 * 16))
                        {
                            break;
                        }
                    }
                }
            }

            actualMinX = Math.Max(0, actualMinX - 10);
            actualMinZ = Math.Max(0, actualMinZ - 10);

            var newWidth = Math.Min(bitmap.Width - actualMinX, actualMaxX - actualMinX + 10);
            var newHeight = Math.Min(bitmap.Height - actualMinZ, actualMaxZ - actualMinZ + 10);

            return bitmap.Crop(new Rectangle(
                    actualMinX,
                    actualMinZ,
                    newWidth,
                    newHeight
                ));
        }

        public Image<Rgba32> GenerateBiomeBitmap()
        {
            var regions = _parser.Regions.ToList();

            var minX = regions.Min(r => r.X);
            var maxX = regions.Max(r => r.X);
            var minZ = regions.Min(r => r.Z);
            var maxZ = regions.Max(r => r.Z);

            var imageWidth = (maxX - minX + 1) * 32 * 16;
            var imageHeight = (maxZ - minZ + 1) * 32 * 16;

            var bitmap = new Image<Rgba32>(imageWidth, imageHeight);

            foreach (var region in regions)
            {
                foreach (var column in region.Columns)
                {
                    foreach (var chunk in column.Chunks)
                    {
                        foreach (var block in chunk.Blocks)
                        {
                            var color = _textureProvider.GetColorForBiome(block.Biome, Rgba32.Aquamarine);

                            var dX = minX * 32 * 16;
                            var dZ = minZ * 32 * 16;

                            bitmap[block.WorldX - dX, block.WorldZ - dZ] = color;
                        }
                    }
                }
            }

            return bitmap;
        }
    }
}
