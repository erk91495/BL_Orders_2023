using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlOrders2023.Exceptions;
using BlOrders2023.Core.Data;
using System.Windows.Forms.Design.Behavior;
using Microsoft.IdentityModel.Tokens;
using ABI.Windows.ApplicationModel.Activation;
using Microsoft.Graphics.Canvas.Text;
using BlOrders2023.Exceptions;

namespace BlOrders2023.Helpers
{

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
            if(item.Scanline == null)
            {
                throw new InvalidBarcodeExcption("Scanline was null");
            }
            IBarcode? barcode = null;
            try
            {
                barcode = new GS1_128Barcode();
                barcode.SetScanline(item.Scanline);
            }
            catch (InvalidBarcodeExcption e) { }

            //TODO try code128

            if (barcode == null)
            {
                throw new InvalidBarcodeExcption("The scanline given did not match a given barcode format");
            }
            else
            {
                barcode.PopuplateProperties(ref item);
            }
        }

        public static bool UpdateBarcode(ref ShippingItem shippingItem)
        {
            if (shippingItem.Scanline != null) 
            {
                //try to set barcode data if exception try next type
                //if (IsGS1128Barcode(shippingItem.Scanline))
                //{
                //    GS1_128Barcode bc = new()
                //    {
                //        Scanline = shippingItem.Scanline,
                //    };
                //    UpdateBarcodeScanline(bc, "3202", "002212");
                //}
                //else
                //{
                //    throw new NotImplementedException();
                //}
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
                string gtin = (item.Product.CompanyCode ?? "90605375") + item.ProductID.ToString("D5");
                AppendG10CheckDigit(ref gtin);
                var scanline = "01" + gtin +
                    "3202" + ((int)((item.PickWeight ?? 0) * 100)).ToString("D6") + "13" + item.PackDate?.ToString("yyMMdd") +
                    "21" + item.PackageSerialNumber;
                item.Scanline = scanline;
                return true;
            }
            return false;
        }

      
        /// <summary>
        /// This method caluculates and appends the check digit for the given gtin in accordance with the gs1 standard
        /// see 
        /// </summary>
        /// <param name="gtin">The gtin to calgulate</param>
        /// <returns> True if the calculation is successful</returns>
        private static bool AppendG10CheckDigit(ref string gtin)
        {
            bool success = false;
            
            if ( gtin.Length <= 13)
            { 
                gtin += CalculateG10CheckDigit(in gtin);
            }            
            return success;
        }

        /// <summary>
        /// Calculates and returns the check digit for the given GTIN 
        /// see https://www.gs1.org/services/how-calculate-check-digit-manually
        /// </summary>
        /// <param name="gtin">the GTIN to calculate</param>
        /// <returns>The check digit for the GTIN</returns>
        private static string CalculateG10CheckDigit(in string gtin)
        {
            int sum = 0;
            //SSCC cheksum is 18 digits
            int j = 0;
            for (int i = gtin.Length - 1; i >= 0; i--)
            {
                char c = gtin[i];
                if (j % 2 == 0)
                {
                    sum += int.Parse(c.ToString()) * 3;
                }
                else
                {
                    sum += int.Parse(c.ToString()) * 1;
                }
                j++;
            }
            int checkDigit = 10 - (sum % 10);
            return checkDigit.ToString();
        }

       




        private static bool UpdateBarcodeScanline(IBarcode barcode, string ai, string value)
        {
            //var index = FindAIStartIndex(barcode, ai);
            //var newScanline = barcode.Scanline[..(index+ai.Length)] + value + barcode.Scanline[]
            return true;
        }

       
    }
}
