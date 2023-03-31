using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data
{
    public interface IShipDetailsTable
    {
        public Task<IEnumerable<ShippingItem>> GetAsync();
        public Task<bool> IsDuplicateScanline(string scanline);
    }
}
