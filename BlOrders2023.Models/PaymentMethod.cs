using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tblPaymentMethods")]
    public class PaymentMethod
    {
        public int PaymentMethodID { get; set; }
        [Column("PaymentMethod")]
        public string? Method { get; set; }
    }
}
