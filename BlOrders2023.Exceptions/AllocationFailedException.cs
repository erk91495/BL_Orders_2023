using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Exceptions;
public class AllocationFailedException : Exception
{
    #region Constructors
    public AllocationFailedException()
    {
    }

    public AllocationFailedException(string message) : base(message)
    {
    }

    public AllocationFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
    #endregion Constructors
}
