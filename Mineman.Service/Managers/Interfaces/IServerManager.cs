using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Managers
{
    public interface IServerManager
    {
        Task Start(Server server);
        Task Stop(Server server);
    }
}