using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
public class Pallet
{
    #region Properties
    public int OrderID { get; set; }
    public int PalletIndex { get; set; }
    public int TotalPallets { get; set; }
    public Dictionary<Product, int> Items { get; set; }
    #endregion Properties

    #region Constructors
    public Pallet(int orderId): this(orderId, 0)
    { }
    public Pallet(int orderId, int index)
    {
        OrderID = orderId;
        PalletIndex = index;
        Items = new Dictionary<Product, int>();
    }
    #endregion Constructors

    #region Methods
    public int GetTotalQuantity() { return Items.Values.Sum(); }
    #endregion Methods
}
