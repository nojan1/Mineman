using ImageSharp;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Models;
using Mineman.Common.Models.Configuration;
using Mineman.Service.Helpers;
using Mineman.WorldParsing;
using Mineman.WorldParsing.MapTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service
{
    public class MapGenerationService
    {
        private const RegionType TARGET_REGION = RegionType.Overworld;

        private readonly IWorldParserFactory _worldParserFactory;
        private readonly IHostingEnvironment _environment;
        private readonly PathOptions _pathOptions;
        private readonly IMapRendererFactory _mapRendererFactory;
        private readonly ILogger<MapGenerationService> _logger;
        private readonly IServiceScopeFactory _serviceFactory;

        public MapGenerationService(IServiceScopeFactory serviceFactory,
                                    IWorldParserFactory worldParserFactory,
                                    IHostingEnvironment environment,
                                    IOptions<PathOptions> pathOptions,
                                    IMapRendererFactory mapRendererFactory,
                                    ILogger<MapGenerationService> logger)
        {
            _serviceFactory = serviceFactory;
            _worldParserFactory = worldParserFactory;
            _environment = environment;
            _pathOptions = pathOptions.Value;
            _mapRendererFactory = mapRendererFactory;
            _logger = logger;
        }

        public Task GenerateForAllWorlds()
        {
            return Task.Run(() =>
            {
                using (var scope = _serviceFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<DatabaseContext>();

                    var worlds = context.Servers.Include(s => s.World)
                                            .Where(s => s.World != null)
                                            .Select(s => s.World)
                                            .ToList();

                    foreach (var world in worlds)
                    {
                        try
                        {
                            _logger.LogInformation($"Generating map for world. ID: {world.ID} Path: '{world.Path}'");

                            var worldPath = _environment.BuildPath(_pathOptions.WorldDirectory, world.Path);
                            var parser = _worldParserFactory.Create(worldPath);

                            if (!parser.GetRegions(TARGET_REGION).Any())
                            {
                                _logger.LogInformation($"World has no region files. Skipping. ID: {world.ID} Path: '{world.Path}'");
                                continue;
                            }

                            var renderer = _mapRendererFactory.Create2DRender(parser);

                            var renderResult = renderer.GenerateBlockBitmap(TARGET_REGION);

                            renderResult.Bitmap.Save(Path.Combine(worldPath, "map.png"))
                                               .Resize(new Size(200, 200))
                                               .Save(Path.Combine(worldPath, "map_thumb.png"));

                            var mapRenderResultDataFilePath = Path.Combine(worldPath, "render-result.json");
                            File.WriteAllText(mapRenderResultDataFilePath, JsonConvert.SerializeObject(new
                            {
                                OffsetX = renderResult.OffsetX,
                                OffsetZ = renderResult.OffsetZ,
                                UnknownBlocks = renderResult.UnknownRenderEntites.OrderByDescending(x => x.Value)
                            }));

                            _logger.LogInformation($"Finished generating map for world. ID: {world.ID} Path: '{world.Path}'");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(new EventId(), e, $"An error ocurred when generating map for world. ID: {world.ID} Path: '{world.Path}' ");
                        }
                    }
                }
            });
        }
    }
}
