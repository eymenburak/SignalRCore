using SignalRCore.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCore.API.Hubs
{
    public interface IProductHub
    {
        Task ReceiveProduct(Product p);
    }
}
