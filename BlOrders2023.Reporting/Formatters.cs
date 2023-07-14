using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Reporting;
internal static class Formatters
{
    public static string PhoneFormatter(string phone)
    {
        if(phone.Length == 10)
        {
            return string.Format("{0:(###)###-####}", Convert.ToInt64(phone));
        }
        return phone;
    }
}
