using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Exceptions
{
    public class InvalidBarcodeExcption : Exception
    {
        public InvalidBarcodeExcption() : base("Invalid Barcode")
        {
        }

        public InvalidBarcodeExcption(string message, string ai = null, string scanline = null, string location = null) : base(message)
        {
            Data.Add("AI", ai);
            Data.Add("Scanline", scanline);
            Data.Add("Location", location);
        }

        public InvalidBarcodeExcption(string message, Exception innerException, string ai = null, string scanline = null, string location = null) : 
            base(message, innerException)
        {
            Data.Add("AI", ai);
            Data.Add("Scanline", scanline);
            Data.Add("Location", location);
        }
    }
}
