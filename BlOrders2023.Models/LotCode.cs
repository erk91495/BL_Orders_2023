using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_LotCodes")]
public class LotCode
{
    [Key]
    [Required]
    [Column("LotCode")]
    public string Lot { get; set; }
}
