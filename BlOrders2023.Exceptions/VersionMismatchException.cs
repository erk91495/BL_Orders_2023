using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Exceptions;
public class VersionMismatchException : Exception
{
    public VersionMismatchException() : base()
    {
    }

    public VersionMismatchException(string message) : base(message)
    {
    }
}
