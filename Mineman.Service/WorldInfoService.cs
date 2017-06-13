using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Service.Helpers;
using Mineman.Service.Models;
using Mineman.WorldParsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service
{
    public class WorldInfoService
    {
        private readonly DatabaseContext _context;
        private readonly IWorldParserFactory _worldParserFactory;
        private readonly IHostingEnvironment _environment;
        private readonly Common.Models.Configuration _configuration;
        private readonly ILogger<WorldInfoService> _logger;

        public WorldInfoService(DatabaseContext context,
                                    IWorldParserFactory worldParserFactory,
                                    IHostingEnvironment environment,
                                    IOptions<Common.Models.Configuration> configuration,
                                    ILogger<WorldInfoService> logger)
        {
            _context = context;
            _worldParserFactory = worldParserFactory;
            _environment = environment;
            _configuration = configuration.Value;
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
                        _logger.LogInformation($"Parsing info for world. ID: {world.ID} Path: '{world.Path}'");

                        var worldPath = _environment.BuildPath(_configuration.WorldDirectory, world.Path);
                        var parser = _worldParserFactory.Create(worldPath);

                        var worldInfo = new WorldInfoModel
                        {
                            Players = parser.Players.Select(p => new PlayerInfoModel
                            {
                                Health = p.Health,
                                X = p.X,
                                Y = p.Y,
                                Z = p.Z,
                                UUID = p.UUID,
                                Inventory = p.Inventory.Select(i => new InventoryItemModel
                                {
                                    Name = i.Id,
                                    Count = i.Count
                                })
                            })
                        };

                        var worldInfoFilePath = _environment.BuildPath(_configuration.WorldDirectory, world.Path, "worldinfo.json");
                        File.WriteAllText(worldInfoFilePath, JsonConvert.SerializeObject(worldInfo));

                        _logger.LogInformation($"Finished parsing info for world. ID: {world.ID} Path: '{world.Path}'");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(new EventId(), e, $"An error ocurred when parsing info for world. ID: {world.ID} Path: '{world.Path}' ");
                    }
                }
            });
        }
    }
}
