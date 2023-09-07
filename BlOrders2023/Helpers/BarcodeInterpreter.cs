using BlOrders2023.Models;
using BlOrders2023.Exceptions;
using BlOrders2023.Models.Helpers;

namespace BlOrders2023.Helpers;


public static class BarcodeInterpreter
{
    /// <summary>
    /// Populates the properties of the given shipping item with the data from the given barcode
    /// </summary>
    /// <param name="barcode">The barcode of the product to be converted</param>
    /// <param name="item">The item to parse the barcode into</param>
    /// <returns>true if the barcode was succesfully parsed</returns>
    /// <exception cref="ProductNotFoundException">If the parsed barcodes product is not in the database</exception>
    public static void ParseBarcode(ref ShippingItem item)
    {
        var isValidBarcodeType = true;
        IBarcode? barcode = null;

        if (item.Scanline == null)
        {
            throw new InvalidBarcodeExcption("Scanline was null");
        }
        
        try
        {
            barcode = new GTIN14Barcode(item.Scanline);
        }
        catch (InvalidBarcodeExcption) 
        {
            //not a GS1 barcode try again
            try
            {
                barcode = new GS1_128Barcode(item.Scanline);
            }
            catch (InvalidBarcodeExcption)
            {
                //not a GS1 barcode try again
                try
                {
                    barcode = new Code128Barcode(item.Scanline);
                }
                catch (InvalidBarcodeExcption)
                {
                    isValidBarcodeType = false;
                }
            }
        }

        if (!isValidBarcodeType || barcode == null)
        {
            throw new UnknownBarcodeFormatException("The scanline given did not match a given barcode format");
        }
        else
        {
            barcode.PopuplateProperties(ref item);
            item.Barcode = barcode;
        }
    }

    public static bool UpdateBarcode(ref ShippingItem shippingItem)
    {
        var isValidBarcodeType = true;
        IBarcode? barcode;
        if (shippingItem.Scanline != null) 
        {

            try
            {
                barcode = new GTIN14Barcode(shippingItem);
                isValidBarcodeType = true;
            }
            catch (InvalidBarcodeExcption) 
            {
                isValidBarcodeType = false;
            }

            if (!isValidBarcodeType)
            {
                try
                {
                    barcode = new GS1_128Barcode(shippingItem);
                    shippingItem.Scanline = barcode.Scanline;
                    isValidBarcodeType = true;
                }
                catch (InvalidBarcodeExcption)
                {
                    isValidBarcodeType = false;
                }
            }

            if (!isValidBarcodeType)
            {
                try
                {
                    barcode = new Code128Barcode(shippingItem);
                    shippingItem.Scanline = barcode.Scanline;
                    isValidBarcodeType = true;
                }
                catch (InvalidBarcodeExcption)
                {
                    isValidBarcodeType = false;
                }
            }

            if (!isValidBarcodeType)
            {
                throw new UnknownBarcodeFormatException();
            }
            else
            {

            }
        }
        return true;
    }

    /// <summary>
    /// Creates a scanline for the given ShippingItem. Overwrites the scanline property. The ShippingItem's
    /// properties must be set
    /// </summary>
    /// <param name="item">The given Shipping Item to create a barcodes from</param>
    /// <returns>true if the barcode is created</returns>
    public static bool SynthesizeBarcode(ref ShippingItem item)
    {

        //TODO: rewrite this to check if properties are null and create a scanline based on the give properties
        // if(Pickweight != 0) { append 3202 + pickweight}
        // if we want to get real fancy allow the user to select ai's and order them then throw errors if values arent populated 
        if (item.PackDate != null && item.Product != null ) {
            //Assumes b&L company code 
            var gtin = (item.Product.CompanyCode ?? "90605375") + item.ProductID.ToString("D5");
            BarcodeHelpers.AppendGTINCheckDigit(ref gtin);
            var scanline = "01" + gtin +
                "3202" + ((int)((item.PickWeight ?? 0) * 100)).ToString("D6") + "13" + item.PackDate?.ToString("yyMMdd") +
                "21" + item.PackageSerialNumber;
            item.Scanline = scanline;
            return true;
        }
        return false;
    }

  



   
}
