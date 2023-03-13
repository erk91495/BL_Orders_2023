﻿using BlOrders2023.Models;
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

        public const char FNC1 = (char)0x1D; //Group Seperator char

        public static bool ParseBarcode(IBarcode barcode, ref ShippingItem item)
        {
            bool success = true;
            if (barcode is GS1_128Barcode gs1Barcode)
            {
                String scanline = new (gs1Barcode.Scanline);
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
                            var ai = match.Substring(0, match.Length - data.Length);
                            scanline = scanline.Substring(match.Length);
                            PopulateAI(ai, data.Value, ref item);
                        }
                        
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("UnsupportedAI {0} {1}",gs1Barcode.Scanline, scanline));
                        success = false;
                        break;
                    }
                }
                return success;
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

        private static bool IsSupportedAI(String key)
        {
            return SupportedAIRegex.ContainsKey(key);
        }

        private static bool PopulateAI(String ai, String data, ref ShippingItem item)
        {
            bool success = false;
            //Is the Ai Supported
            if(SupportedAIRegex.ContainsKey(ai))
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
                    var prodCode = int.Parse(data.Substring(8, 5));                    
                    var product = App.BLDatabase.Products.Get(prodCode).FirstOrDefault();
                    if (product != null)
                    {
                        item.Product = product;
                        item.ProductID = prodCode;
                    }
                    else
                    {
                        throw new Exception("Product Not Found");
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
                }
                //Packaging data (YYMMDD)
                else if (ai.Equals("13"))
                {
                    item.PackDate = DateTime.ParseExact(data,"yyMMdd",null);
                }
                //Serial Number
                else if (ai.Equals("21"))
                {
                    //TODO: GS1 Supports non numeric characters
                    item.PackageSerialNumber = int.Parse(data);
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
    }
}
