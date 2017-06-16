using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Service.Helpers;
using Mineman.Service.Models;
using Mineman.WorldParsing;
using Mineman.WorldParsing.Blocks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private const RegionType TARGET_REGION = RegionType.Overworld;

        private readonly IWorldParserFactory _worldParserFactory;
        private readonly IHostingEnvironment _environment;
        private readonly Common.Models.Configuration _configuration;
        private readonly ILogger<WorldInfoService> _logger;
        private readonly IServiceScopeFactory _serviceFactory;

        public WorldInfoService(IServiceScopeFactory serviceFactory,
                                IWorldParserFactory worldParserFactory,
                                IHostingEnvironment environment,
                                IOptions<Common.Models.Configuration> configuration,
                                ILogger<WorldInfoService> logger)
        {
            _serviceFactory = serviceFactory;
            _worldParserFactory = worldParserFactory;
            _environment = environment;
            _configuration = configuration.Value;
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
                            _logger.LogInformation($"Parsing info for world. ID: {world.ID} Path: '{world.Path}'");

                            var worldPath = _environment.BuildPath(_configuration.WorldDirectory, world.Path);
                            var parser = _worldParserFactory.Create(worldPath);

                            var worldInfo = new WorldInfoModel();
                            PopulatePlayers(parser, worldInfo);
                            PopulateBlockEntities(parser, worldInfo);

                            worldInfo.SpawnX = parser.Level?.SpawnX ?? 0;
                            worldInfo.SpawnY = parser.Level?.SpawnY ?? 0;
                            worldInfo.SpawnZ = parser.Level?.SpawnZ ?? 0;

                            var worldInfoFilePath = _environment.BuildPath(_configuration.WorldDirectory, world.Path, "worldinfo.json");
                            File.WriteAllText(worldInfoFilePath, JsonConvert.SerializeObject(worldInfo));

                            _logger.LogInformation($"Finished parsing info for world. ID: {world.ID} Path: '{world.Path}'");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(new EventId(), e, $"An error ocurred when parsing info for world. ID: {world.ID} Path: '{world.Path}' ");
                        }
                    }
                }
            });
        }

        private void PopulateBlockEntities(IWorldParser parser, WorldInfoModel worldInfo)
        {
            var chests = new List<ChestInfoModel>();
            var signs = new List<SignInfoModel>();

            foreach (var region in parser.GetRegions(TARGET_REGION))
            {
                foreach (var column in region.Columns)
                {
                    foreach (var chunk in column.Chunks)
                    {
                        foreach (var block in chunk.Blocks)
                        {
                            if (block is Chest)
                            {
                                var chest = block as Chest;

                                chests.Add(new ChestInfoModel
                                {
                                    X = chest.WorldX,
                                    Y = chest.WorldY,
                                    Z = chest.WorldZ,
                                    Items = chest.Items.Select(i => new InventoryItemModel
                                    {
                                        Name = i.Id,
                                        Count = i.Count
                                    }).ToList()
                                });
                            }
                            else if (block is Sign)
                            {
                                var sign = block as Sign;
                                string textRendered = string.Join("\n", sign.TextLines.Select(s =>
                                {
                                    if (s.Contains('{') && s.Contains('}') && s.Contains("\"text\":"))
                                    {
                                        var obj = JObject.Parse(s);
                                        return obj.GetValue("text").ToString();
                                    }
                                    else
                                    {
                                        return s;
                                    }
                                }));

                                signs.Add(new SignInfoModel
                                {
                                    X = sign.WorldX,
                                    Y = sign.WorldY,
                                    Z = sign.WorldZ,
                                    Text = textRendered
                                });
                            }
                        }
                    }
                }
            }

            worldInfo.Chests = chests;
            worldInfo.Signs = signs;
        }

        private void PopulatePlayers(IWorldParser parser, WorldInfoModel worldInfo)
        {
            worldInfo.Players = parser.Players.Select(p => new PlayerInfoModel
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
            }).ToList();
        }
    }
}
