using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Services
{

    public static class BarcodeInterpreter
    {
        //Keep In mind as we add fields here we will also need to support them when we parse the fields into product data
        public readonly static Dictionary<String, Regex> SupportedAIRegex = new()
        {
            {"00", new Regex("^00(\\d{18})$")},                                                                               //SSCC
            {"01", new Regex("^01(\\d{14})")},                                                                               //GTIN
            {"02", new Regex("^02(\\d{14})$ ")},                                                                             //GTIN OF CONTAINED ITEMS
            {"10", new Regex("^10([\\x21-\\x22\\x25-\\x2F\\x30-\\x39\\x3A-\\x3F\\x41-\\x5A\\x5F\\x61-\\x7A]{0,20})")},       //LOT CODE
            {"13", new Regex("^13(\\d{6})")},                                                                                //PACKAGING DATE
            {"21", new Regex("^21([\\x21-\\x22\\x25-\\x2F\\x30-\\x39\\x3A-\\x3F\\x41-\\x5A\\x5F\\x61-\\x7A]{0,20})")},       //SERIAL NUMBER
            {"320",new Regex("^320(\\d)(\\d{6})")},                                                                          //NET WEIGHT (LBS.)
        };

        public const char FNC1 = (char)0x1D; //GS char

        public static bool ParseBarcode(IBarcode barcode, ref ShippingItem item)
        {
            bool success = true;
            if (barcode is GS1_128Barcode code)
            {
                String scanline = new (code.Scanline);
                while(scanline != "\r")
                {
                    Regex? re = null;
                    if (IsSupportedAI(scanline.Substring(0, 2)))
                    {
                        re = SupportedAIRegex.GetValueOrDefault(scanline.Substring(0,2));
                    }
                    else if (IsSupportedAI(scanline.Substring(0, 3)))
                    {
                        re = SupportedAIRegex.GetValueOrDefault(scanline.Substring(0, 3));
                    }
                    if(re != null)
                    {
                        var matches = re.Match(scanline);
                        if (matches.Success)
                        {
                            var groups = matches.Groups;
                            var match = matches.Value;
                            var data = groups[groups.Count-1];
                            scanline = scanline.Substring(match.Length);
                        }
                        
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("UnsupportedAI {0} {1}",code.Scanline, scanline));
                        success = false;
                        break;
                    }
                }
                return success;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static bool IsSupportedAI(String key)
        {
            return SupportedAIRegex.ContainsKey(key);
        }


    }
}
