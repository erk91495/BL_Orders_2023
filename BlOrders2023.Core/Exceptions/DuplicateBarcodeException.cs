using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Exceptions
{
    public class DuplicateBarcodeException : Exception
    {
        public DuplicateBarcodeException() : base("Duplicate Scanline")
        {
        }

        public DuplicateBarcodeException(string message, string scanline = null) : base(message)
        {
            Data.Add("Scanline", scanline);
        }

        public DuplicateBarcodeException(string message, Exception innerException, string scanline = null) :
            base(message, innerException)
        {
            Data.Add("Scanline", scanline);
        }
    }
}
