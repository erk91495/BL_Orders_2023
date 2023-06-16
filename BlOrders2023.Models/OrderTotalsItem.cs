using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Keyless]
    public class OrderTotalsItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public int? TotalQuantity { get; set; }
        public int? TotalReceived { get; set; }
    }
}
