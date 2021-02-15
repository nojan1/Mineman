using Microsoft.AspNetCore.SignalR;
using Mineman.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Hubs
{
    public class InfoHub : Hub<IInfoClient> { }
}
