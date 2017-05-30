using Mineman.WorldParsing.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Mineman.WorldParsing.MapTools
{
    public class MapRenderer2D
    {
        private static readonly Dictionary<BiomeType, Color> biomeColorTable = new Dictionary<BiomeType, Color>()
        {
            { BiomeType.Ocean, Color.Blue },
            { BiomeType.Plains, Color.Lime },
            { BiomeType.Desert, Color.LightYellow },
            { BiomeType.Extreme_Hills, Color.Gray },
            { BiomeType.Forest, Color.DarkGreen },
            { BiomeType.Taiga, Color.LightSteelBlue },
            { BiomeType.Swampland, Color.Brown },
            { BiomeType.River, Color.LightBlue },
            { BiomeType.Hell, Color.Red },
            { BiomeType.Sky, Color.WhiteSmoke },
            { BiomeType.Frozen_Ocean, Color.Cyan },
            { BiomeType.Frozen_River, Color.Cyan },
            { BiomeType.Ice_Plains, Color.LightCyan },
            { BiomeType.Ice_Mountains, Color.LightCyan },
            { BiomeType.Mushroom_Island, Color.LightPink },
            { BiomeType.Mushroom_Island_Shore, Color.LightPink },
            { BiomeType.Beach, Color.SandyBrown },
            { BiomeType.Desert_Hills, Color.Sienna },
            { BiomeType.Forest_Hills, Color.Salmon },
            { BiomeType.Taiga_Hills, Color.RosyBrown },
            { BiomeType.Extreme_Hills_Edge, Color.Olive },
            { BiomeType.Jungle, Color.Moccasin },
            { BiomeType.Jungle_Hills, Color.NavajoWhite },
        };

        private static readonly Dictionary<int, Color> blockColorTable = new Dictionary<int, Color>()
        {
            {1, Color.Gray},
            {2, Color.LightGreen },
            {3, Color.Brown },
            {4, Color.DarkGray },
            {5, Color.SaddleBrown },
            {8, Color.Blue },
            {9, Color.Blue },
            {10, Color.OrangeRed },
            {11, Color.Orange },
            {12, Color.Beige },
            {13, Color.SlateGray },
            {16, Color.DimGray },
            {18, Color.DarkGreen },
            {31, Color.LightGreen },
            {32, Color.Beige },
            {35, Color.CornflowerBlue },
            {37, Color.LightGoldenrodYellow },
            {38, Color.MistyRose },
            {43, Color.Gray },
            {44, Color.Gray },
            {48, Color.PaleGreen },
            {50, Color.Yellow },
            {53, Color.BurlyWood },
            {59, Color.ForestGreen },
            {67, Color.Gray },
            {78, Color.GhostWhite },
            {79, Color.LightCyan },
            {80, Color.GhostWhite },
            {85, Color.Brown },
            {98, Color.LightSlateGray },
            {107, Color.Brown },
            {108, Color.Crimson },
            {109, Color.Gray },
            {111, Color.Olive },
            {114, Color.Maroon },
            {125, Color.Brown },
            {126, Color.Brown },
            {128, Color.SandyBrown },
            {134, Color.BurlyWood },
            {135, Color.BurlyWood },
            {136, Color.BurlyWood },
            {156, Color.Gainsboro },
            {163, Color.BurlyWood },
            {164, Color.BurlyWood },
            {174, Color.Cyan },
            {175, Color.LawnGreen },
            {180, Color.IndianRed },
            {183, Color.Brown },
            {184, Color.Brown },
            {185, Color.Brown },
            {186, Color.Brown },
            {187, Color.Brown },
            {188, Color.Brown },
            {189, Color.Brown },
            {190, Color.Brown },
            {191, Color.Brown },
            {192, Color.Brown },
            {212, Color.DarkCyan }
        };

        private readonly WorldParser _parser;

        public MapRenderer2D(WorldParser parser)
        {
            _parser = parser;
        }

        public Bitmap GenerateBlockBitmap()
        {
            var regions = _parser.Regions.ToList();

            var minX = regions.Min(r => r.X);
            var maxX = regions.Max(r => r.X);
            var minZ = regions.Min(r => r.Z);
            var maxZ = regions.Max(r => r.Z);

            var imageWidth = (maxX - minX + 1) * 32 * 16;
            var imageHeight = (maxZ - minZ + 1) * 32 * 16;

            var bitmap = new Bitmap(imageWidth, imageHeight);
            var populated = new List<(int, int)>(16 * 16);

            var biomes = new List<BiomeType>();

            foreach (var region in regions)
            {
                foreach (var column in region.Columns)
                {
                    populated.Clear();
                    foreach (var chunk in column.Chunks.OrderByDescending(c => c.YOrder))
                    {
                        foreach (var block in chunk.Blocks.OrderByDescending(b => b.WorldY))
                        {
                            if (block.Id == 0)
                                continue;

                            if (populated.Contains((block.WorldX, block.WorldZ)))
                                continue;

                            if(!biomes.Contains(block.Biome)) biomes.Add(block.Biome);

                            var color = blockColorTable.ContainsKey(block.Id) ? blockColorTable[block.Id]
                                                                              : Color.HotPink;
                            
                            populated.Add((block.WorldX, block.WorldZ));

                            var dX = minX * 32 * 16;
                            var dZ = minZ * 32 * 16;

                            bitmap.SetPixel(block.WorldX - dX, block.WorldZ - dZ, color);
                        }

                        if (populated.Count == (16 * 16))
                        {
                            break;
                        }
                    }
                }
            }

            return bitmap;
        }

        public Bitmap GenerateBiomeBitmap()
        {
            var regions = _parser.Regions.ToList();

            var minX = regions.Min(r => r.X);
            var maxX = regions.Max(r => r.X);
            var minZ = regions.Min(r => r.Z);
            var maxZ = regions.Max(r => r.Z);

            var imageWidth = (maxX - minX + 1) * 32 * 16;
            var imageHeight = (maxZ - minZ + 1) * 32 * 16;

            var bitmap = new Bitmap(imageWidth, imageHeight);

            foreach (var region in regions)
            {
                foreach (var column in region.Columns)
                {
                    foreach (var chunk in column.Chunks)
                    {
                        foreach (var block in chunk.Blocks)
                        {
                            Color color;
                            if (biomeColorTable.ContainsKey(block.Biome))
                            {
                                color = biomeColorTable[block.Biome];
                            }
                            else
                            {
                                color = Color.Aquamarine;
                            }

                            var dX = minX * 32 * 16;
                            var dZ = minZ * 32 * 16;

                            bitmap.SetPixel(block.WorldX - dX, block.WorldZ - dZ, color);
                        }
                    }
                }
            }

            return bitmap;
        }
    }
}
