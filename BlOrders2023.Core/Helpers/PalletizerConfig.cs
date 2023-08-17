using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Helpers;
public class PalletizerConfig
{
    public bool SingleItemPerPallet { get; set; }
    public int BBoxesPerPallet { get; set; } = 80;
    public int CBoxesPerPallet { get; set; } = 108;
    public int MixedBoxesPerPallet { get; set; } = 60;

    public int BreastBoxesPerPallet {get; set; } = 140;
}
