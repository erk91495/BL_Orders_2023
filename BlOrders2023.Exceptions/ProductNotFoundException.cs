using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() : base("Product Code not found in the database")
        {
        }

        public ProductNotFoundException(string message) : base(message)
        {
        }

        public ProductNotFoundException(string message, int ProductID) : base(message)
        {
            Data.Add("ProductID", ProductID);
        }

        public ProductNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ProductNotFoundException(string message, Exception innerException , int ProductID) : base(message, innerException)
        {
            Data.Add("ProductID", ProductID);
        }
    }
}
