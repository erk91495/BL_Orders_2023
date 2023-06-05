using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    public class Code128Barcode : IBarcode
    {
        public Code128Barcode(ShippingItem item) : base(item)
        {
        }

        public Code128Barcode(string scanline) : base(scanline)
        {
        }

        public override string Scanline => throw new NotImplementedException();

        public override bool PopuplateProperties(ref ShippingItem item)
        {
            throw new NotImplementedException();
        }
    }
}
