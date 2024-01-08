using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Models
{
    [Table("tblPayments")]
    public class Payment: ObservableObject
    {
        #region Fields
        private string? _notes;
        private decimal? paymentAmount;
        #endregion Fields
        [Key]
        [Column("PaymentID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }
        public int? OrderId { get; set; }
        public int? CustId { get; set; }
        public decimal? PaymentAmount
        {
            get => Math.Round(paymentAmount ?? 0,2); 
            set 
            {
                if(value is null)
                {
                    SetProperty( ref paymentAmount, value);
                }
                else
                {
                    SetProperty(ref paymentAmount, Math.Round((decimal)value, 2));
                }
                
            }
        }
        public DateTime? PaymentDate { get; set; }
        public string? CreditCardNumber { get; set; }
        public string? CardholdersName { get; set; }
        public DateTime? CreditCardExpDate { get; set; }
        public int? PaymentMethodID {  get; set; }
        public string? Notes 
        { 
            get => _notes; 
            set
            {
                if (value.IsNullOrEmpty())
                {
                    _notes = null;
                }
                else
                {
                    _notes = value;
                }
            } 
        }
        public string? CheckNumber { get; set; }
        
        [ForeignKey(nameof(PaymentMethodID))]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [ForeignKey(nameof(CustId))]
        public virtual WholesaleCustomer? Customer { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; } = null;
    }
}
