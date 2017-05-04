using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Helpers
{
    public static class DockerQueryHelper
    {
        public static async Task<IList<ContainerListResponse>> GetContainers(IDockerClient dockerClient)
        {
            return await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    { "label", new Dictionary<string, bool> { { "creator=mineman", true } }  }
                }
            });
        }

        public static async Task<ContainerListResponse> GetContainer(IDockerClient dockerClient, string containerID)
        {
            return (await GetContainers(dockerClient)).FirstOrDefault(c => c.ID == containerID);
        }
    }
}
