using ImageSharp;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Models;
using Mineman.Service.Helpers;
using Mineman.WorldParsing;
using Mineman.WorldParsing.MapTools;
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
        private readonly DatabaseContext _context;
        private readonly IWorldParserFactory _worldParserFactory;
        private readonly IHostingEnvironment _environment;
        private readonly Common.Models.Configuration _configuration;
        private readonly IMapRendererFactory _mapRendererFactory;
        private readonly ILogger<MapGenerationService> _logger;

        public MapGenerationService(DatabaseContext context,
                                    IWorldParserFactory worldParserFactory,
                                    IHostingEnvironment environment,
                                    IOptions<Common.Models.Configuration> configuration,
                                    IMapRendererFactory mapRendererFactory,
                                    ILogger<MapGenerationService> logger)
        {
            _context = context;
            _worldParserFactory = worldParserFactory;
            _environment = environment;
            _configuration = configuration.Value;
            _mapRendererFactory = mapRendererFactory;
            _logger = logger;
        }

        public Task GenerateForAllWorlds()
        {
            return Task.Run(() =>
            {
                var worlds = _context.Servers.Include(s => s.World)
                                        .Where(s => s.World != null)
                                        .Select(s => s.World)
                                        .ToList();

                foreach (var world in worlds)
                {
                    try
                    {
                        _logger.LogInformation($"Generating map for world. ID: {world.ID} Path: '{world.Path}'");

                        var worldPath = _environment.BuildPath(_configuration.WorldDirectory, world.Path);
                        var parser = _worldParserFactory.Create(worldPath);

                        if (!parser.GetRegions(RegionType.Overworld).Any())
                        {
                            _logger.LogInformation($"World has no region files. Skipping. ID: {world.ID} Path: '{world.Path}'");
                            continue;
                        }

                        var renderer = _mapRendererFactory.Create2DRender(parser);

                        renderer.GenerateBlockBitmap(RegionType.Overworld)
                            .Save(Path.Combine(worldPath, "map.png"))
                            .Resize(new Size(200, 200))
                            .Save(Path.Combine(worldPath, "map_thumb.png"));

                        _logger.LogInformation($"Finished generating map for world. ID: {world.ID} Path: '{world.Path}'");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(new EventId(), e, $"An error ocurred when generating map for world. ID: {world.ID} Path: '{world.Path}' ");
                    }
                }
            });
        }
    }
}
