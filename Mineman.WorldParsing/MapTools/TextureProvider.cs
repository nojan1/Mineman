using ImageSharp;
using Mineman.WorldParsing.Blocks;
using Mineman.WorldParsing.MapTools.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mineman.WorldParsing.MapTools
{
    public class TextureProvider : ITextureProvider
    {
        private static readonly Dictionary<BiomeType, Rgba32> biomeColorTable = new Dictionary<BiomeType, Rgba32>()
        {
            { BiomeType.Ocean, Rgba32.Blue },
            { BiomeType.Plains, Rgba32.Lime },
            { BiomeType.Desert, Rgba32.LightYellow },
            { BiomeType.Extreme_Hills, Rgba32.Gray },
            { BiomeType.Forest, Rgba32.DarkGreen },
            { BiomeType.Taiga, Rgba32.LightSteelBlue },
            { BiomeType.Swampland, Rgba32.Brown },
            { BiomeType.River, Rgba32.LightBlue },
            { BiomeType.Hell, Rgba32.Red },
            { BiomeType.Sky, Rgba32.WhiteSmoke },
            { BiomeType.Frozen_Ocean, Rgba32.Cyan },
            { BiomeType.Frozen_River, Rgba32.Cyan },
            { BiomeType.Ice_Plains, Rgba32.LightCyan },
            { BiomeType.Ice_Mountains, Rgba32.LightCyan },
            { BiomeType.Mushroom_Island, Rgba32.LightPink },
            { BiomeType.Mushroom_Island_Shore, Rgba32.LightPink },
            { BiomeType.Beach, Rgba32.SandyBrown },
            { BiomeType.Desert_Hills, Rgba32.Sienna },
            { BiomeType.Forest_Hills, Rgba32.Salmon },
            { BiomeType.Taiga_Hills, Rgba32.RosyBrown },
            { BiomeType.Extreme_Hills_Edge, Rgba32.Olive },
            { BiomeType.Jungle, Rgba32.Moccasin },
            { BiomeType.Jungle_Hills, Rgba32.NavajoWhite },
        };

        private static readonly Dictionary<string, Rgba32> blockColorTable = new Dictionary<string, Rgba32>()
        {
            {"1", Rgba32.Gray},
            {"2", Rgba32.LightGreen },
            {"3", Rgba32.Sienna },
            {"4", Rgba32.DarkGray },
            {"5", Rgba32.SaddleBrown },
            {"8", Rgba32.Blue },
            {"9", Rgba32.Blue },
            {"10", Rgba32.OrangeRed },
            {"11", Rgba32.Orange },
            {"12", Rgba32.Beige },
            {"13", Rgba32.SlateGray },
            {"16", Rgba32.DimGray },
            {"18", Rgba32.DarkGreen },
            {"31", Rgba32.LightGreen },
            {"32", Rgba32.Beige },
            {"35", Rgba32.CornflowerBlue },
            {"37", Rgba32.LightGoldenrodYellow },
            {"38", Rgba32.MistyRose },
            {"43", Rgba32.Gray },
            {"44", Rgba32.Gray },
            {"48", Rgba32.PaleGreen },
            {"50", Rgba32.Yellow },
            {"53", Rgba32.BurlyWood },
            {"59", Rgba32.ForestGreen },
            {"67", Rgba32.Gray },
            {"78", Rgba32.GhostWhite },
            {"79", Rgba32.LightCyan },
            {"80", Rgba32.GhostWhite },
            {"85", Rgba32.Brown },
            {"98", Rgba32.LightSlateGray },
            {"107", Rgba32.Brown },
            {"108", Rgba32.Crimson },
            {"109", Rgba32.Gray },
            {"111", Rgba32.Olive },
            {"114", Rgba32.Maroon },
            {"125", Rgba32.Brown },
            {"126", Rgba32.Brown },
            {"128", Rgba32.SandyBrown },
            {"134", Rgba32.BurlyWood },
            {"135", Rgba32.BurlyWood },
            {"136", Rgba32.BurlyWood },
            {"156", Rgba32.Gainsboro },
            {"163", Rgba32.BurlyWood },
            {"164", Rgba32.BurlyWood },
            {"174", Rgba32.Cyan },
            {"175", Rgba32.LawnGreen },
            {"180", Rgba32.IndianRed },
            {"183", Rgba32.Brown },
            {"184", Rgba32.Brown },
            {"185", Rgba32.Brown },
            {"186", Rgba32.Brown },
            {"187", Rgba32.Brown },
            {"188", Rgba32.Brown },
            {"189", Rgba32.Brown },
            {"190", Rgba32.Brown },
            {"191", Rgba32.Brown },
            {"192", Rgba32.Brown },
            {"212", Rgba32.DarkCyan }
        };

        private string _resourcePackPath;

        public TextureProvider(string resourcePackPath)
        {
            _resourcePackPath = resourcePackPath;
        }

        public Rgba32 GetColorForBlock(Block block, Rgba32 fallbackColor)
        {
            if (string.IsNullOrEmpty(_resourcePackPath))
            {
                if (blockColorTable.ContainsKey(block.Id))
                {
                    return blockColorTable[block.Id];
                }
                else if (blockColorTable.ContainsKey(block.BaseId.ToString()))
                {
                    return blockColorTable[block.BaseId.ToString()];
                }
                else
                {
                    return fallbackColor;
                }
            }
            throw new NotImplementedException();
            //Parse the resource stuff
            return ColorFromBlockTexture(block.Code, fallbackColor);
        }

        public Rgba32 GetColorForBiome(BiomeType biomeType, Rgba32 fallbackColor)
        {
            if (string.IsNullOrEmpty(_resourcePackPath))
            {
                if (!biomeColorTable.ContainsKey(biomeType))
                    return fallbackColor;

                return biomeColorTable[biomeType];
            }

            //Parse the resource stuff

            return Rgba32.Black;
        }
        private Rgba32 ColorFromBlockTexture(string code, Rgba32 fallbackColor)
        {
            var blockStateFilePath = Path.Combine(_resourcePackPath, "assets", "minecraft", "blockstates", $"{code}.json");
            if (!File.Exists(blockStateFilePath))
            {
                return fallbackColor;
            }

            var blockStateJson = File.ReadAllText(blockStateFilePath);
            var blockState = ParseBlockstate(blockStateJson);
            return Rgba32.Aqua;
        }

        private BlockState ParseBlockstate(string blockStateJson)
        {
            var root = JObject.Parse(blockStateJson);
            var variants = root["variants"];

            return new BlockState();
        }

        public Rgba32 GetBlocklightColor()
        {
            return Rgba32.DarkOrange;
        }
    }
}
