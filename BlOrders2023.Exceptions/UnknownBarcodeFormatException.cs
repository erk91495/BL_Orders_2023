using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Exceptions
{
    public class UnknownBarcodeFormatException : Exception
    {
        public UnknownBarcodeFormatException() : base("Unknown Barcode Format")
        {
        }

        public UnknownBarcodeFormatException(string message, string scanline = null) : base(message)
        {
            Data.Add("Scanline", scanline);
        }

        public UnknownBarcodeFormatException(string message, Exception innerException, string scanline = null) : 
            base(message, innerException)
        {
            Data.Add("Scanline", scanline);
        }
    }
}
