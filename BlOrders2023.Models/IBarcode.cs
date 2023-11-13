using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{

    public abstract class IBarcode
    {
        protected IBarcode(ShippingItem item) { }
        protected IBarcode(string scanline) { }
        public abstract string Scanline { get; }
        public abstract bool PopuplateProperties(ref ShippingItem item);
    }
}
