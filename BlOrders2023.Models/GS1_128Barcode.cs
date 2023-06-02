using BlOrders2023.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BlOrders2023.Models
{
    public class GS1_128Barcode : IBarcode
    {
        public const char FNC1 = (char)0x1D; //Group Seperator char
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
            {"3202",new Regex("^320(\\d)(\\d{6})")},                                                                          //NET WEIGHT (LBS.)
        };

        private Dictionary<string, string> _AIValues = new();
        private string? _scanline;
        public string? Scanline
        {
            get => _scanline;
            private set => _scanline = value;
        }

        public bool PopuplateProperties(ref ShippingItem item)
        {
            foreach(var ai in _AIValues.Keys) 
            {
                if(!PopulateProperty(ai, _AIValues[ai], ref item))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetScanline(string scanline)
        {
            _scanline = scanline;
            ParseScanline();
        }

        private void ParseScanline()
        {
            var scanline = _scanline.Trim('\r', '\n');
            //TODO: break if no matches found
            while (!scanline.IsNullOrEmpty())
            {
                Regex? re = null;
                re = SupportedAIRegex.GetValueOrDefault(GetNextAI(scanline));

                if (re != null)
                {
                    var matches = re.Match(scanline);
                    if (matches.Success)
                    {
                        var groups = matches.Groups;
                        var match = matches.Value;
                        var data = groups[groups.Count - 1];
                        var ai = match[..^data.Length];
                        scanline = scanline[match.Length..];
                        _AIValues.Add(ai,data.Value);
                    }
                }
                else
                {
                    Debug.WriteLine(String.Format("UnsupportedAI {0} at {1}", Scanline, scanline));
                    throw new InvalidBarcodeExcption("Invalid Barcode", null, Scanline, scanline);
                }

                //todo check if this works. need to find fnc1 and remove it 
                if (scanline.StartsWith(FNC1))
                {
                    scanline = scanline[1..];
                }
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
            else if (key.Length == 3)
            {
                return SupportedAIRegex.ContainsKey(key.Substring(0, 3));
            }
            else if (key.Length == 4)
            {
                return SupportedAIRegex.ContainsKey(key.Substring(0, 4));
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Populates a ShippingItem property with the given ai and data
        /// </summary>
        /// <param name="ai">The AI to populate</param>
        /// <param name="data">The data for the given AI</param>
        /// <param name="item">The ShippingItem to populate</param>
        /// <returns>True if the population was successful</returns>
        private static bool PopulateProperty(String ai, String data, ref ShippingItem item)
        {
            bool success = false;
            //Is the Ai Supported
            if (IsSupportedAI(ai))
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
                    item.ProductID = prodCode;
                    success = true;
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
                    item.PackDate = DateTime.ParseExact(data, "yyMMdd", null);
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

        private static bool IsGS1128Barcode(string scanline)
        {
            return IsSupportedAI(scanline[..2]) || IsSupportedAI(scanline[..3]);
        }

        private static string GetNextAI(string scanline)
        {
            if (scanline.Length >= 2 && IsSupportedAI(scanline[..2]))
            {
                return scanline[..2];
            }
            else if (scanline.Length >= 3 && IsSupportedAI(scanline[..3]))
            {
                return scanline[..3];
            }
            else if (scanline.Length >= 4 && IsSupportedAI(scanline[..4]))
            {
                return scanline[..4];
            }
            else
            {
                return "";
            }
        }

        private static int FindAIStartIndex(IBarcode barcode, string ai)
        {
            var scanline = barcode.Scanline;
            if (scanline != null)
            {
                while (scanline != "\r" && !scanline.IsNullOrEmpty())
                {
                    var currentAI = GetNextAI(scanline);
                    var re = SupportedAIRegex.GetValueOrDefault(currentAI);
                    if (currentAI == ai)
                    {
                        return barcode.Scanline!.Length - scanline.Length;
                    }

                    if (re != null && !currentAI.IsNullOrEmpty())
                    {
                        var matches = re.Match(scanline);
                        if (matches.Success)
                        {
                            var match = matches.Value;
                            scanline = scanline[match.Length..];
                        }
                    }

                    if (scanline.StartsWith(FNC1))
                    {
                        scanline = scanline[1..];
                    }


                }
            }
            return -1;
        }
    }
}
