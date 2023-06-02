using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    public class Code128Barcode : IBarcode
    {
        public string? Scanline { get; private set;}

        public bool PopuplateProperties(ref ShippingItem item)
        {
            throw new NotImplementedException();
        }

        public void SetScanline(string scanline)
        {
            throw new NotImplementedException();
        }
    }
}
