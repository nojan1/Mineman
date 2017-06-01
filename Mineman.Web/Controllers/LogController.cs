using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mineman.Service.Repositories;
using Docker.DotNet;
using System.Threading;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Mineman.Web.Controllers
{
    [Route("api/server/log")]
    [Authorize]
    public class LogController : Controller
    {
        private readonly IServerRepository _serverRepository;
        private readonly IDockerClient _dockerClient;

        public LogController(IServerRepository serverRepository,
                             IDockerClient dockerClient)
        {
            _serverRepository = serverRepository;
            _dockerClient = dockerClient;
        }

        [HttpGet("{serverId}")]
        public async Task<IActionResult> GetLog(int serverId)
        {
            var server = await _serverRepository.Get(serverId);
            if (server == null)
            {
                return BadRequest();
            }

            var response = await _dockerClient.Containers.GetContainerLogsAsync(server.ContainerID,
                    new Docker.DotNet.Models.ContainerLogsParameters
                    {
                        ShowStderr = true,
                        ShowStdout = true
                    }, CancellationToken.None);

            using (var reader = new StreamReader(response))
            {
                var data = await reader.ReadToEndAsync();

                var log = string.Join("\n", data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(l =>
                              {
                                  if(l.Length > 8)
                                  {
                                      var bytes = l.ToCharArray()
                                                   .Select(c => (byte)Convert.ToInt32(c))
                                                   .Skip(8)
                                                   .ToArray();

                                      return Encoding.ASCII.GetString(bytes);
                                  }
                                  else
                                  {
                                      return l;
                                  }
                              }));

                return Ok(new
                {
                    Timestamp = DateTimeOffset.Now,
                    Log = log
                });
            }
        }
    }
}
