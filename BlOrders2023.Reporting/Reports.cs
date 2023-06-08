using BlOrders2023.Reporting.ReportClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Reporting
{
    public static class Reports
    {
        public static readonly Collection<Type> AvalibleReports = new()
        { 
            typeof(WholesaleInvoice), 
            typeof(WholesaleOrderPickupRecap) 
        }; 
    }

    public enum InputTypes
    {
        NO_INPUT,
        WHOLESALE_CUSTOMER,
        ORDER,
        WHOLESALE_CUSTOMER_LIST,
        ORDER_LIST
    }
}
