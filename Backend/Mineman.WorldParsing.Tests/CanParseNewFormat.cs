using Mineman.WorldParsing.Tests.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mineman.WorldParsing.Tests
{
    public class CanParseNewFormat
    {
        [Fact]
        public void Test()
        {
            var parser = new WorldParser(@"../../../../Testfiles/world-1.14");

            foreach (var region in parser.GetRegions(RegionType.Overworld))
            {
                foreach (var column in region.Columns)
                {
                    if (!column.TerrainPopulated)
                        continue;

                    foreach (var chunk in column.Chunks)
                    {
                        foreach (var block in chunk.Blocks)
                        {
                            Console.WriteLine($"X: {block.WorldX} Z: {block.WorldZ} Y: {block.WorldY} - {block.Code}");
                        }
                    }
                }
            }
        }

        [Fact]
        public void GenerateNewWorld()
        {
            var parser = new WorldParser(@"../../../../Testfiles/world-1.14");

            var textureProvider = new MapTools.TextureProvider(new MapTools.Models.TextureOptions {
                    BlockColorsFilePath = "../../../../Mineman.WorldParsing/Resources/blockcolors.json",
                    BiomeColorsFilePath = "../../../../Mineman.WorldParsing/Resources/biomecolors.json"
            });

            var factory = new MapTools.MapRendererFactory(textureProvider);
            var renderer = factory.Create2DRender(parser, null);

            var x = renderer.GenerateBlockBitmap(RegionType.Overworld);
            x.Bitmap.SaveAsJpeg(System.IO.File.OpenWrite("C:/users/nojan/Desktop/test.jpg"));
        }
    }
}
