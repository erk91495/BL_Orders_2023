using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlOrders2023.Core.Exceptions;

namespace BlOrders2023.Helpers
{

    public static class BarcodeInterpreter
    {
        //Keep In mind as we add fields here we will also need to support them when we parse the fields into product data
        private readonly static Dictionary<string, Regex> SupportedAIRegex = new()
        {
            //Keep In mind as we add fields here we will also need to support them when we parse the fields into product data
            {"00", new Regex("^00(\\d{18})$")},                                                                               //SSCC
            {"01", new Regex("^01(\\d{14})")},                                                                               //GTIN
            {"02", new Regex("^02(\\d{14})$ ")},                                                                             //GTIN OF CONTAINED ITEMS
            {"10", new Regex("^10([\\x21-\\x22\\x25-\\x2F\\x30-\\x39\\x3A-\\x3F\\x41-\\x5A\\x5F\\x61-\\x7A]{0,20})")},       //LOT CODE
            {"13", new Regex("^13(\\d{6})")},                                                                                //PACKAGING DATE
            {"21", new Regex("^21([\\x21-\\x22\\x25-\\x2F\\x30-\\x39\\x3A-\\x3F\\x41-\\x5A\\x5F\\x61-\\x7A]{0,20})")},       //SERIAL NUMBER
            {"320",new Regex("^320(\\d)(\\d{6})")},                                                                          //NET WEIGHT (LBS.)
        };

        public const char FNC1 = (char)0x1D; //Group Seperator char

        /// <summary>
        /// Populates the properties of the given shipping item with the data from the given barcode
        /// </summary>
        /// <param name="barcode">The barcode of the product to be converted</param>
        /// <param name="item">The item to parse the barcode into</param>
        /// <returns>true if the barcode was succesfully parsed</returns>
        /// <exception cref="ProductNotFoundException">If the parsed barcodes product is not in the database</exception>
        public static void ParseBarcode(IBarcode barcode, ref ShippingItem item)
        {
            if (barcode is GS1_128Barcode gs1Barcode)
            {
                String scanline = new (gs1Barcode.Scanline);
                item.Scanline = scanline.Trim('\r', '\n');
                //TODO: break if no matches found
                while(scanline != "\r")
                {
                    Regex? re = null;
                    if (IsSupportedAI(scanline[..2]))
                    {
                        re = SupportedAIRegex.GetValueOrDefault(scanline[..2]);
                    }
                    else if (IsSupportedAI(scanline[..3]))
                    {
                        re = SupportedAIRegex.GetValueOrDefault(scanline[..3]);
                    }
                    if(re != null)
                    {
                        var matches = re.Match(scanline);
                        if (matches.Success)
                        {
                            var groups = matches.Groups;
                            var match = matches.Value;
                            var data = groups[groups.Count-1];
                            var ai = match[..^data.Length];
                            scanline = scanline[match.Length..];
                            if(!PopulateAI(ai, data.Value, ref item))
                            {
                                throw new InvalidBarcodeExcption("Invalid Barcode", ai, barcode.Scanline, scanline);
                            }
                        } 
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("UnsupportedAI {0} at {1}", gs1Barcode.Scanline, scanline));
                        throw new InvalidBarcodeExcption("Invalid Barcode", null , barcode.Scanline, scanline);
                    }

                    //todo check if this works. need to find fnc1 and remove it 
                    if (scanline.StartsWith(FNC1))
                    {
                        scanline = scanline[1..];
                    }
                }
            }
            else if(barcode is Code128Barcode)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Check if the given AI is supported by the interpreter
        /// </summary>
        /// <param name="key">The AI to be checked</param>
        /// <returns>True if the AI is supported</returns>
        private static bool IsSupportedAI(String key)
        {
            if (key.Length == 2)
            {
                return SupportedAIRegex.ContainsKey(key);
            }
            else if (key.Length == 4 || key.Length == 3)
            {
                return SupportedAIRegex.ContainsKey(key.Substring(0,3));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Populates a ShippingItem with the given ai and data
        /// </summary>
        /// <param name="ai">The AI to populate</param>
        /// <param name="data">The data for the given AI</param>
        /// <param name="item">The ShippingItem to populate</param>
        /// <returns>True if the population was successful</returns>
        /// <exception cref="ProductNotFoundException">Thrown when the given Product Code AI (01) is not in the database</exception>
        private static bool PopulateAI(String ai, String data, ref ShippingItem item)
        {
            bool success = false;
            //Is the Ai Supported
            if(IsSupportedAI(ai))
            {
                //Serial Shipping Container Code (SSCC)
                if (ai.Equals("00"))
                {
                    throw new NotImplementedException(); 
                }
                //Global Trade Item Number(GTIN)
                else if (ai.Equals("01"))
                {
                    // Product code is 5 digits
                    var prodCode = int.Parse(data[8..^1]);
                    var product = App.BLDatabase.Products.Get(prodCode).FirstOrDefault();
                    if (product != null)
                    {
                        item.Product = product;
                        item.ProductID = prodCode;
                        success = true;
                    }
                    else
                    {
                        throw new ProductNotFoundException(String.Format("Product {0} Not Found", prodCode), prodCode);
                    }
                }
                //Global Trade Item Number (GTIN) of contained trade items
                else if (ai.Equals("02"))
                {
                    throw new NotImplementedException();
                }
                //Batch or lot number
                else if (ai.Equals("10"))
                {
                    var lotCode = data;
                    success = true;
                }
                //Packaging data (YYMMDD)
                else if (ai.Equals("13"))
                {
                    item.PackDate = DateTime.ParseExact(data,"yyMMdd",null);
                    success = true;
                }
                //Serial Number
                else if (ai.Equals("21"))
                {
                    //TODO: GS1 Supports non numeric characters
                    item.PackageSerialNumber = data;
                    success = true;
                }
                //Net Weight in lbs. 
                else if (ai.StartsWith("320"))
                {
                    float netWt = float.Parse(data);
                    //The fourth digit of this AI indicates the number of decimal places (see GS1 General Specifications for details).
                    double power = float.Parse(ai.Substring(ai.Length - 2));
                    float multiplier = (float)Math.Pow(10, power);
                    netWt /= multiplier;
                    item.PickWeight = netWt;
                    success = true;
                }
            }
            return success;
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
    }
}
