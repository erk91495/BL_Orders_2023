using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Models;
[Table("tbl_WsaleMessage")]
public class WholesaleCustomerNote
{
    #region Fields
    private int _custId;
    private string? _msgText;
    #endregion Fields

    #region Properties
    [Key]
    public int CustId
    {
        get => _custId;
        set => _custId = value;
    }

    public string? MsgText
    {
        get => _msgText;
        set => _msgText = value.IsNullOrEmpty() ? null : value;
    }
    [ForeignKey(nameof(CustId))]
    public virtual WholesaleCustomer Customer { get; set; }
    #endregion Properties
}
