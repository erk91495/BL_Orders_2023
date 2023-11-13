using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Contracts;
internal interface IPalletizerConfig
{
    public bool SingleItemPerPallet{ get; set; }
    public int BBoxesPerPallet { get; set; }
    public int CBoxesPerPallet { get; set; }
    public int MixedBoxesPerPallet { get; set; }

    public int BreastBoxesPerPallet { get; set; }
}
