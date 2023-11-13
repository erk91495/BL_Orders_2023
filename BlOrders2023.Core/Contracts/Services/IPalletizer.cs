using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Contracts.Services;
internal interface IPalletizer
{
   public Task<IEnumerable<Pallet>> PalletizeAsync();
}
