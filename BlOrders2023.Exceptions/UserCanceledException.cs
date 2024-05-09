using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Exceptions;
public class UserCanceledException : Exception
{
    public UserCanceledException()
    {
    }

    public UserCanceledException(string message) : base(message)
    {
    }

    public UserCanceledException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
