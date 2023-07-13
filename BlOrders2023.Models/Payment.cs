using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tblPayments")]
    public class Payment
    {
        [Key]
        [Column("PaymentID")]
        public int PaymentID { get; set; }
        public int OrderId { get; set; }
        public int CustId { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? CreditCardNumber { get; set; }
        public string? CardholdersName { get; set; }
        public DateTime? CreditCardExpDate { get; set; }
        public int? PaymentMethodID {  get; set; }
        public string? Notes { get; set; }
        public string? CheckNumber { get; set; }
        
        [ForeignKey(nameof(PaymentMethodID))]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [ForeignKey(nameof(CustId))]
        public virtual WholesaleCustomer? Customer { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;
    }
}
