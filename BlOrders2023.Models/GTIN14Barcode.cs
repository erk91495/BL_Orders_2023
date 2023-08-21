using BlOrders2023.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Models;
public class GTIN14Barcode : IBarcode
{
    #region Properties
    private string _scanline;
    #endregion Properties
    public GTIN14Barcode(ShippingItem item) : base(item)
    {
        if (!(item.Product.FixedPrice ?? false))
        {
            throw new InvalidBarcodeExcption("Invalid Shipping Item");
        }

        _scanline = item.Scanline ?? "";

    }

    public GTIN14Barcode(string scanline) : base(scanline)
    {
        _scanline = scanline;
    }

    public override string Scanline => _scanline;

    public override bool PopuplateProperties(ref ShippingItem item)
    {
        item.ProductID = int.Parse(_scanline[8..^1]);
        return true;
    }
}
