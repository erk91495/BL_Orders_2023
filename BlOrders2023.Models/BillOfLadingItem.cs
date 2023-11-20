using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
public class BillOfLadingItem
{
    public int ProductID { get; set; }
    public int? NumberOfPallets { get; set; }
    public int NumberOfCases { get; set; }
    public string ProductName {get; set; } = string.Empty;
    public decimal? GrossWt => NumberOfPallets != null ? (int)(NumberOfPallets * 60) + (decimal)(NumberOfCases * 1.2) + NetWt : null;
    public decimal NetWt { get; set; }
}
