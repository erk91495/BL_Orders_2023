using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{

    public enum OrderStatus
    {
        Ordered,
        Filling,
        Filled,
        Printed,
        Paid,
        Complete,

    }
}
