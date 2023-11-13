using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Contracts;

namespace BlOrders2023.Core.Helpers;
public class PalletizerConfig : IPalletizerConfig
{
    public bool SingleItemPerPallet { get; set; }
    public int BBoxesPerPallet { get; set; } = 108;
    public int CBoxesPerPallet { get; set; } = 80;
    public int MixedBoxesPerPallet { get; set; } = 80;
    public int BreastBoxesPerPallet {get; set; } = 140;
}
