using ImageSharp;
using ImageSharp.PixelFormats;
using ImageSharp.PixelFormats.PixelBlenders;
using Mineman.WorldParsing.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.WorldParsing.MapTools
{
    public class MapRenderer2D : IMapRenderer2D
    {
        private static int[] transparentBlocks = new int[] { 20, 8, 9 };

        private readonly IWorldParser _parser;
        private readonly ITextureProvider _textureProvider;

        public MapRenderer2D(IWorldParser parser,
                             ITextureProvider textureProvider)
        {
            _parser = parser;
            _textureProvider = textureProvider;
        }

        public Image<Rgba32> GenerateBlockBitmap(RegionType regionType)
        {
            var regions = _parser.GetRegions(regionType).ToList();

            var minX = regions.Min(r => r.X);
            var maxX = regions.Max(r => r.X);
            var minZ = regions.Min(r => r.Z);
            var maxZ = regions.Max(r => r.Z);

            var imageWidth = (maxX - minX + 1) * 32 * 16;
            var imageHeight = (maxZ - minZ + 1) * 32 * 16;

            var dX = minX * 32 * 16;
            var dZ = minZ * 32 * 16;

            var bitmap = new Image<Rgba32>(imageWidth, imageHeight);

            int actualMinX = Int32.MaxValue, actualMaxX = 0, actualMinZ = Int32.MaxValue, actualMaxZ = 0;
            bool blockRendered = false;

            //foreach (var region in regions)
            Parallel.ForEach(regions, region =>
            {
                var populated = new List<(int, int)>(16 * 16);
                var lastBaseColor = new Dictionary<(int, int), Rgba32>(16 * 16);

                foreach (var column in region.Columns)
                {
                    populated.Clear();
                    lastBaseColor.Clear();

                    foreach (var chunk in column.Chunks.OrderByDescending(c => c.YOrder))
                    {
                        foreach (var block in chunk.Blocks.OrderByDescending(b => b.WorldY))
                        {
                            //In image coordinates
                            var x = block.WorldX - dX;
                            var z = block.WorldZ - dZ;

                            //Update image edge extens if neccessary
                            actualMinX = Math.Min(actualMinX, x);
                            actualMaxX = Math.Max(actualMaxX, x);
                            actualMinZ = Math.Min(actualMinZ, z);
                            actualMaxZ = Math.Max(actualMaxZ, z);

                            //Do not draw air
                            if (block.BaseId == 0)
                                continue;

                            //Coordinates populated and completed, ignore
                            if (populated.Contains((x, z)))
                                continue;

                            var baseColor = _textureProvider.GetColorForBlock(block, Rgba32.HotPink);

                            Rgba32 color;
                            if (lastBaseColor.ContainsKey((x, z)))
                            {
                                //Handle non top blocks in transparent stacks

                                if (baseColor == lastBaseColor[(x, z)])
                                {
                                    continue;
                                }

                                if (block.SkyLight == 0 && bitmap[x, z].A > 0)
                                {
                                    populated.Add((x, z));
                                    continue;
                                }

                                color = BlendBlocks(bitmap[x, z], baseColor, block.SkyLight);
                                lastBaseColor[(x, z)] = baseColor;
                            }
                            else
                            {
                                //First block in stack, it can either be transparent or not
                                color = baseColor;
                            }

                            //Handle blocklight; ie torches, lava etc
                            if (block.BlockLight > 0)
                            {
                                //color = Screen(color, _textureProvider.GetBlocklightColor(), block.BlockLight / 512);
                                color = BlendBlocks(color, _textureProvider.GetBlocklightColor(), block.BlockLight / 2);
                            }

                            if (transparentBlocks.Contains(block.BaseId))
                            {
                                //This is a transparent block stack, save this color for future (next block in -Y order) reference.
                                //This is also the flag that a stack is transparent
                                lastBaseColor[(x, z)] = baseColor;
                            }
                            else
                            {
                                //Not transparent, mark as populated
                                populated.Add((x, z));
                            }

                            blockRendered = true;
                            bitmap[x, z] = color;
                        }

                        if (populated.Count == (16 * 16))
                        {
                            break;
                        }
                    }
                }
            });

            if (!blockRendered)
            {
                return bitmap;
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


        public Image<Rgba32> GenerateBiomeBitmap(RegionType regionType)
        {
            var regions = _parser.GetRegions(regionType).ToList();

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

        private Rgba32 Screen(Rgba32 colorA, Rgba32 colorB, int alpha)
        {
            Func<float, float, float> screenImpl = (a, b) =>
            {
                var value = 1 - ((1 - a) * (1 - b));

                return a + (value * (1 - alpha));
            };

            var afterScreen = new Rgba32(
                    screenImpl(colorA.R / 255f, colorB.R / 255f) * 255f,
                    screenImpl(colorA.G / 255f, colorB.G / 255f) * 255f,
                    screenImpl(colorA.B / 255f, colorB.B / 255f) * 255f
                );

            return afterScreen;
        }


        private Rgba32 BlendBlocks(Rgba32 colorA, Rgba32 colorB, float skylight)
        {
            Func<float, float, float> worker = (a, b) =>
            {
                var alpha = skylight > 16 ? 16 : skylight;

                var value = ((a * (16 - alpha - 1)) + (b * (alpha - 1))) / 16f;

                //float value;
                //if (a < 127)
                //{
                //    value = 2 * a * b;
                //}
                //else
                //{
                //    value = 1 - 2 + 2 * a + 2 * b - 2 * a * b;
                //}

                //TODO: Alpha merge

                return value;

                //var value =  a + (b * (1 - alpha));

                //return value < 10 ? 10 : value > 230 ? 230 : value;
            };

            var result = new Rgba32(
                    worker(colorA.R, colorB.R),
                    worker(colorA.G, colorB.G),
                    worker(colorA.B, colorB.B)
                );

            return result;
        }
    }
}
