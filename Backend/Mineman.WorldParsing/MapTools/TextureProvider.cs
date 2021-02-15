using Mineman.WorldParsing.Blocks;
using Mineman.WorldParsing.MapTools.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using SixLabors.ImageSharp;

namespace Mineman.WorldParsing.MapTools
{
    public class TextureProvider : ITextureProvider
    {
        private Dictionary<BiomeType, Color> biomeColorTable;
        private Dictionary<string, Color> blockColorTable;

        private readonly TextureOptions _textureOptions;
        public TextureProvider(TextureOptions textureOptions)
        {
            _textureOptions = textureOptions;

            LoadBlockColorFile();
            LoadBiomeColorFile();
        }

        public Color GetColorForBlock(Block block, Color fallbackColor)
        {
            if (blockColorTable.ContainsKey(block.Id))
            {
                return blockColorTable[block.Id];
            }
            else if (blockColorTable.ContainsKey(block.BaseId.ToString()))
            {
                return blockColorTable[block.BaseId.ToString()];
            }
            else if (blockColorTable.ContainsKey("*"))
            {
                //Default color from color file
                return blockColorTable["*"];
            }
            else
            {
                //Default hardcoded color
                return fallbackColor;
            }
        }

        public Color GetColorForBiome(BiomeType biomeType, Color fallbackColor)
        {
            if (!biomeColorTable.ContainsKey(biomeType))
                return fallbackColor;

            return biomeColorTable[biomeType];
        }

        public Color GetBlocklightColor()
        {
            return Color.DarkOrange;
        }

        private void LoadBlockColorFile()
        {
            blockColorTable = DeserializeLines(_textureOptions.BlockColorsFilePath)
                .ToDictionary(x => x.Key,
                              x => ColorFromString(x.Value));
        }

        private void LoadBiomeColorFile()
        {
            biomeColorTable = DeserializeLines(_textureOptions.BiomeColorsFilePath)
                .ToDictionary(x => (BiomeType)Enum.Parse(typeof(BiomeType), x.Key, true),
                              x => ColorFromString(x.Value));
        }

        private static FieldInfo[] rgba32Fields;
        private static Color ColorFromString(string colorString)
        {
            if(rgba32Fields == null)
            {
                rgba32Fields = typeof(Color).GetFields();
            }

            if (colorString.StartsWith("#"))
            {
                return SixLabors.ImageSharp.PixelFormats.Rgba32.ParseHex(colorString);
            }
            else
            {
                var field = rgba32Fields.First(p => p.Name.ToLower() == colorString.ToLower());
                return (Color)field.GetValue(null);
            }
        }

        private Dictionary<string, string> DeserializeLines(string filePath)
        {
            return JsonConvert.DeserializeObject<List<JObject>>(File.ReadAllText(filePath))
                    .ToDictionary(x => x.Properties().First().Name, x => x.PropertyValues().First().ToString());
        }
    }
}
