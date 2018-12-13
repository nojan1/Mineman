using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.PixelFormats.PixelBlenders;
using Mineman.WorldParsing.Blocks;
using Mineman.WorldParsing.MapTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SixLabors.Primitives;

namespace Mineman.WorldParsing.MapTools
{
    public class MapRenderer2D : IMapRenderer2D
    {
        private static int[] transparentBlocks = new int[] { 20, 8, 9, 95 };

        private readonly IWorldParser _parser;
        private readonly ITextureProvider _textureProvider;
        private readonly IMapColumnCacheProvider _cacheProvider;

        public MapRenderer2D(IWorldParser parser,
                             ITextureProvider textureProvider,
                             IMapColumnCacheProvider cacheProvider)
        {
            _parser = parser;
            _textureProvider = textureProvider;
            _cacheProvider = cacheProvider;
        }

        public RenderReturnModel GenerateBlockBitmap(RegionType regionType)
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
            var unknownBlocks = new Dictionary<string, int>();

            int actualMinX = Int32.MaxValue, actualMaxX = 0, actualMinZ = Int32.MaxValue, actualMaxZ = 0;
            bool blockRendered = false;

            //foreach (var region in regions)
            Parallel.ForEach(regions, region =>
            {
                var regionCache = _cacheProvider.GetRegionMapCache(region.X, region.Z) ?? new RegionMapCache();

                var populated = new List<(int, int)>(16 * 16);
                var lastBaseColor = new Dictionary<(int, int), Rgba32>(16 * 16);

                foreach (var column in region.Columns)
                {
                    if (regionCache.ColumnCacheIsStale(column.XWorld, column.ZWorld, column.Modified))
                    {
                        regionCache.UpdateTimestamp(column.XWorld, column.ZWorld, column.Modified);

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
                                if (baseColor == Rgba32.HotPink)
                                {
                                    var key = $"{block.Code} ({block.Id})";
                                    unknownBlocks[key] = unknownBlocks.ContainsKey(key) ? unknownBlocks[key] + 1 : 1;
                                }

                                Rgba32 color;
                                if (lastBaseColor.ContainsKey((x, z)))
                                {
                                    //Handle non top blocks in transparent stacks

                                    if (baseColor == lastBaseColor[(x, z)])
                                    {
                                        continue;
                                    }

                                    if (!transparentBlocks.Contains(block.BaseId) && block.SkyLight == 0 && block.BlockLight == 0 && bitmap[x, z].A > 0)
                                    {
                                        populated.Add((x, z));
                                        continue;
                                    }

                                    color = BlendBlocks(bitmap[x, z], baseColor, Math.Max(block.SkyLight, (byte)(block.BlockLight / 2)));
                                    lastBaseColor[(x, z)] = baseColor;
                                }
                                else
                                {
                                    //First block in stack, it can either be transparent or not
                                    color = baseColor;
                                }

                                //Handle blocklight; ie torches, lava etc
                                //if (block.BlockLight > 0)
                                //{
                                //    color = BlendBlocks(color, _textureProvider.GetBlocklightColor(), (byte)(block.BlockLight / 2));
                                //}

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

                                regionCache.SetBlockColor(column.XWorld, column.ZWorld, block.WorldX, block.WorldZ, color);
                            }

                            if (populated.Count == (16 * 16))
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        var cachedColumn = regionCache.GetColumn(column.XWorld, column.ZWorld);
                        foreach(var cachedBlockColor in cachedColumn.BlockColors)
                        {
                            int x = cachedBlockColor.Coordinates.Item1 - dX;
                            int z = cachedBlockColor.Coordinates.Item2 - dZ;

                            blockRendered = true;
                            bitmap[x, z] = new Rgba32(cachedBlockColor.Argb);

                            //Update image edge extens if neccessary
                            actualMinX = Math.Min(actualMinX, x);
                            actualMaxX = Math.Max(actualMaxX, x);
                            actualMinZ = Math.Min(actualMinZ, z);
                            actualMaxZ = Math.Max(actualMaxZ, z);
                        }
                    }
                }

                _cacheProvider.SaveRegionMapCache(region.X, region.Z, regionCache);
            });

            if (!blockRendered)
            {
                return new RenderReturnModel
                {
                    Bitmap = bitmap,
                    OffsetX = dX,
                    OffsetZ = dZ,
                    UnknownRenderEntites = unknownBlocks
                };
            }

            actualMinX = Math.Max(0, actualMinX - 10);
            actualMinZ = Math.Max(0, actualMinZ - 10);

            var newWidth = Math.Min(bitmap.Width - actualMinX, actualMaxX - actualMinX + 10);
            var newHeight = Math.Min(bitmap.Height - actualMinZ, actualMaxZ - actualMinZ + 10);

            bitmap.Mutate(x => x.Crop(new Rectangle(
                    actualMinX,
                    actualMinZ,
                    newWidth,
                    newHeight
                )));

            return new RenderReturnModel
            {
                Bitmap = bitmap,
                OffsetX = -dX - actualMinX,
                OffsetZ = -dZ - actualMinZ,
                UnknownRenderEntites = unknownBlocks
            };
        }


        public RenderReturnModel GenerateBiomeBitmap(RegionType regionType)
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

            foreach (var region in regions)
            {
                foreach (var column in region.Columns)
                {
                    foreach (var chunk in column.Chunks)
                    {
                        foreach (var block in chunk.Blocks)
                        {
                            var color = _textureProvider.GetColorForBiome(block.Biome, Rgba32.Aquamarine);

                            bitmap[block.WorldX - dX, block.WorldZ - dZ] = color;
                        }
                    }
                }
            }

            return new RenderReturnModel
            {
                Bitmap = bitmap,
                OffsetX = dX,
                OffsetZ = dZ,
                UnknownRenderEntites = new Dictionary<string, int>()
            };
        }

        private Rgba32 BlendBlocks(Rgba32 colorA, Rgba32 colorB, byte skylight)
        {
            var alpha = (float)skylight;

            Func<float, float, float> blendColorComponent = (a, b) => ((a / 255f) * (15 - alpha) + (b / 255f) * alpha) / 15f;

            var result = new Rgba32(
                    blendColorComponent(colorA.R, colorB.R),
                    blendColorComponent(colorA.G, colorB.G),
                    blendColorComponent(colorA.B, colorB.B)
                );

            return result;
        }
    }
}
