using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tblCustomerWholesale")]
    public class WholesaleCustomer
    {
        [Key]
        public int CustID { get; set; }
        public string CustomerName { get; set; } = "";
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string Phone { get; set; } = "";
        public string? PhoneExt { get; set; }
        public string? Phone_2 { get; set; }
        public string? Phone2Ext { get; set; }
        public string? Buyer { get; set; }
        public string? LastPurchase { get; set; }
        public string? Fax { get; set; }
        public string? Class { get; set; }
        public short Grocer { get; set; }
        public short Inactive { get; set; }
        public string? Email { get; set; }
        [Column("Gift-Grocer")]
        public string? Gift_Grocer { get; set; }
        public bool? SingleProdPerPallet { get; set; }
        public List<Order> orders { get; set; } = new List<Order>();
    }
}
