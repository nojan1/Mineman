using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Models;
using Mineman.Service.Repositories;
using Mineman.WorldParsing;
using Mineman.WorldParsing.Blocks;
using Mineman.WorldParsing.MapTools;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mineman.Service.Tests
{
    public class temp
    {
        [Fact]
        public void lsdgj()
        {
            //var databaseOptions = new DbContextOptionsBuilder<DatabaseContext>()
            //    .UseInMemoryDatabase()
            //    .Options;

            //var context = new DatabaseContext(databaseOptions);

            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddTransient<DatabaseContext>(services => context);

            //var scopeFactory = new Mock<IServiceScopeFactory>();
            //scopeFactory.Setup(x => x.CreateScope())
            //    .Returns(() =>
            //    {
            //        var scope = new Mock<IServiceScope>();
            //        scope.SetupGet(s => s.ServiceProvider)
            //            .Returns(new DefaultServiceProviderFactory().CreateServiceProvider(serviceCollection));

            //        return scope.Object;
            //    });

            //var hostingEnvironment = new Mock<IHostingEnvironment>();
            //var options = new Mock<IOptions<Configuration>>();
            //options.SetupGet(x => x.Value)
            //    .Returns(new Configuration
            //    {
            //        WorldDirectory = @"C:\Users\hedlundn\Desktop\worlds\"
            //    });

            //var logger = new Mock<ILogger<WorldInfoService>>();

            //context.Servers.Add(new Common.Database.Models.Server
            //{
            //    World = new Common.Database.Models.World
            //    {
            //        Path = "604b2298e89a4733ad54f607ab83948e"
            //    }
            //});
            //context.SaveChanges();

            //var worldInfoService = new WorldInfoService(scopeFactory.Object,
            //                                            new WorldParserFactory(),
            //                                            hostingEnvironment.Object,
            //                                            options.Object,
            //                                            logger.Object);

            //worldInfoService.GenerateForAllWorlds().Wait();

            //var region = parser.GetRegions(RegionType.Overworld).First();
            //var column = region.Columns.First();
            //var blockEnties = region.Columns.SelectMany(c => c.BlockEntities).ToList();
            //var chunk = column.Chunks.First();
            //var blocks = chunk.Blocks.ToList();

            //var blocks = parser.GetRegions(RegionType.Overworld).SelectMany(r => r.Columns)
            //                           .SelectMany(c => c.Entities)
            //                           .ToList();

            //var worldPath = @"C:\Users\hedlundn\Desktop\worlds\survivial-world";
            //var parser = new WorldParser(worldPath);
            //var fileBasedRegionCache = new FileBasedRegionCache(worldPath);

            //var renderer = new MapRenderer2D(parser, new TextureProvider(new WorldParsing.MapTools.Models.TextureOptions
            //{
            //    BlockColorsFilePath = "../../../../Mineman.WorldParsing/Resources/blockcolors.json",
            //    BiomeColorsFilePath = "../../../../Mineman.WorldParsing/Resources/biomecolors.json"
            //}), fileBasedRegionCache);
            ////renderer.GenerateBiomeBitmap(RegionType.Overworld).Bitmap.Save(@"C:\Users\hedlundn\Desktop\map-overworld-biome.png");

            //var result = renderer.GenerateBlockBitmap(RegionType.Overworld);

            //result.Bitmap.Save(@"C:\Users\hedlundn\Desktop\map-overworld.png");
        }
    }
}
