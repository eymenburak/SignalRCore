using Microsoft.AspNetCore.SignalR;
using SignalRCore.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCore.API.Hubs
{
    public class ProductHub : Hub<IProductHub>
    {
        public async Task SendProduct(Product p)
        {
            await Clients.All.ReceiveProduct(p);
        }
    }
}
