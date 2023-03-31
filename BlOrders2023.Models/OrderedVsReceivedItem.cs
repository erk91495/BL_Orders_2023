using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    public class OrderedVsReceivedItem
    {
        public int ProductID { get; set; }
        public int Ordered {  get; set; }
        public int Received { get; set; }
    }
}
