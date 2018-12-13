using SixLabors.ImageSharp;
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

namespace Mineman.WorldParsing.MapTools
{
    public class TextureProvider : ITextureProvider
    {
        private Dictionary<BiomeType, Rgba32> biomeColorTable;
        private Dictionary<string, Rgba32> blockColorTable;

        private readonly TextureOptions _textureOptions;
        public TextureProvider(TextureOptions textureOptions)
        {
            _textureOptions = textureOptions;

            LoadBlockColorFile();
            LoadBiomeColorFile();
        }

        public Rgba32 GetColorForBlock(Block block, Rgba32 fallbackColor)
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

        public Rgba32 GetColorForBiome(BiomeType biomeType, Rgba32 fallbackColor)
        {
            if (!biomeColorTable.ContainsKey(biomeType))
                return fallbackColor;

            return biomeColorTable[biomeType];
        }

        public Rgba32 GetBlocklightColor()
        {
            return Rgba32.DarkOrange;
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
        private static Rgba32 ColorFromString(string colorString)
        {
            if(rgba32Fields == null)
            {
                rgba32Fields = typeof(Rgba32).GetFields();
            }

            if (colorString.StartsWith("#"))
            {
                return Rgba32.FromHex(colorString);
            }
            else
            {
                var field = rgba32Fields.First(p => p.Name.ToLower() == colorString.ToLower());
                return (Rgba32)field.GetValue(null);
            }
        }

        private Dictionary<string, string> DeserializeLines(string filePath)
        {
            return JsonConvert.DeserializeObject<List<JObject>>(File.ReadAllText(filePath))
                    .ToDictionary(x => x.Properties().First().Name, x => x.PropertyValues().First().ToString());
        }
    }
}
