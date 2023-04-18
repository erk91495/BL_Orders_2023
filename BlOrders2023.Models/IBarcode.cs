using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{

    public interface IBarcode
    {
        public string? Scanline { get; set; }
    }
}
