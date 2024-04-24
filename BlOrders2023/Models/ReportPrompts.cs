using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
public static class ReportPrompts
{
    public enum PromptTypes
    {
        None,
        OrderID,
        Date,
        DateRange,
        Customer,
        Customers,
        CustomersAndOrders,
        BillOfLading,
        ProductCategories,
        AuditTrail,
        OrderByDateOrAlphabetical
    }
}
