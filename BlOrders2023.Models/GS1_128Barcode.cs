using BlOrders2023.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.RegularExpressions;
using BlOrders2023.Models.Helpers;
namespace BlOrders2023.Models;



public class GS1_128Barcode : IBarcode
{
    public const char FNC1 = (char)0x1D; //Group Seperator char

    #region Properties
    public override string Scanline => GenerateScanline();
    #endregion Properties

    #region Fields 
    //Keep In mind as we add fields here we will also need to support them when we parse the fields into product data
    private static readonly Dictionary<string, Regex> SupportedAIRegex = new()
    {
        //Keep In mind as we add fields here we will also need to support them when we parse the fields into product data
        {"00", new Regex("^00(\\d{18})$")},                                                                               //SSCC
        {"01", new Regex("^01(\\d{14})")},                                                                               //GTIN
        {"02", new Regex("^02(\\d{14})$ ")},                                                                             //GTIN OF CONTAINED ITEMS
        {"10", new Regex("^10([\\x21-\\x22\\x25-\\x2F\\x30-\\x39\\x3A-\\x3F\\x41-\\x5A\\x5F\\x61-\\x7A]{0,20})")},       //LOT CODE
        {"11", new Regex("^11(\\d{6})")},
        {"13", new Regex("^13(\\d{6})")},                                                                                //PACKAGING DATE
        {"21", new Regex("^21([\\x21-\\x22\\x25-\\x2F\\x30-\\x39\\x3A-\\x3F\\x41-\\x5A\\x5F\\x61-\\x7A]{0,20})")},       //SERIAL NUMBER
        {"3202",new Regex("^320(\\d)(\\d{6})")},                                                                          //NET WEIGHT (LBS.)
    };

    private Dictionary<string, string> _AIValues = new();
    private readonly string? _scanline;
    #endregion Fields

    #region Constructors
    public GS1_128Barcode(ShippingItem item) : base(item)
    {
        //TODO Need a way to determine order of AI's in a barcode. Then i can get rud
        _scanline = item.Scanline;
        ParseScanline();
        //Scanline may be inconsisten with what the Items propertes are. 
        ParseShippingItem(item);
    }

    public GS1_128Barcode(string scanline) : base(scanline)
    {
        _scanline = scanline;
        ParseScanline();
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Populates the given ShippingItem's Properties from the original scanline values 
    /// DOES NOT UPDATE ShippingItemm.Scanline
    /// </summary>
    /// <param name="item">The shipping item to update</param>
    /// <returns>true if all AI's were successfully coonverted to properties</returns>
    public override  bool PopuplateProperties(ref ShippingItem item)
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


    /// <summary>
    /// Updates the barcode scanline and shipping item scanline based on the properties of the Shipping Item
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidBarcodeExcption">Throws Invalid Barcode Exception if a non supported ai is found</exception>
    private void ParseShippingItem(ShippingItem item)
    {
        foreach(var ai in _AIValues.Keys)
        {
            //GTIN
            if (ai.Equals("01"))
            {
                var newGTIN = _AIValues["01"][..^6] + $"{item.ProductID:D5}";
                BarcodeHelpers.AppendGTINCheckDigit(ref newGTIN);
                UpdateAI("01", newGTIN);
            }
            //LOT CODE
            else if (ai.Equals("10"))
            {
                //we dont save this ai so just carry it through to the scanline
                UpdateAI(ai, _AIValues[ai]);
            }
            //PRODUCTION DATE
            else if (ai.Equals("11"))
            {
                UpdateAI(ai, (item.PackDate ?? DateTime.Now).ToString("yyMMdd"));
            }
            //PACKAGING DATE
            else if (ai.Equals("13"))
            {
                UpdateAI(ai, (item.PackDate ?? DateTime.Now).ToString("yyMMdd"));
            }
            //SERIAL NUMBER
            else if (ai.Equals("21"))
            {
                UpdateAI(ai, item.PackageSerialNumber ?? "");
            }
            //NET WEIGHT
            else if (ai.StartsWith("320"))
            {
                var netWT = item.PickWeight;
                double power = float.Parse(ai.Substring(ai.Length - 2));
                var multiplier = (float)Math.Pow(10, power);
                netWT *= multiplier;
                UpdateAI(ai,(netWT ?? 0f).ToString("0#####"));
            }
            //Forgot to write code for that ai i guess
            else
            {
                throw new InvalidBarcodeExcption("Unsupported AI");
            }
        }
    }

    /// <summary>
    /// Uses _AIValues to generate a new Scanline
    /// </summary>
    private string GenerateScanline()
    {
        var newScanline = "";
        foreach(var ai in _AIValues.Keys)
        {
            newScanline += ai + _AIValues[ai];
        }
        return newScanline;
    }

    private void UpdateAI(string ai, string value)
    {
        if (_AIValues.ContainsKey(ai))
        {
            _AIValues[ai] = value;
        }
        else
        {
            _AIValues.Add(ai, value);
        }
    }

    /// <summary>
    /// Populates _AIValues with AI keys and Data Values from the given scanline
    /// </summary>
    /// <exception cref="InvalidBarcodeExcption">Throws InvalidBarcodeExcption if an 
    /// AI cannot be found in the Supported AI Regex</exception>
    private void ParseScanline()
    {
        var scanline = _scanline!.Trim();
        _AIValues.Clear();
        //TODO: break if no matches found
        while (!scanline.IsNullOrEmpty())
        {
            Regex? re;
            re = SupportedAIRegex!.GetValueOrDefault(GetNextAI(scanline));

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
                else
                {
                    Debug.WriteLine(String.Format("Dev messed up regex AI {0} at {1}", Scanline, scanline));
                    throw new InvalidBarcodeExcption("Regex Match Not Made", re.ToString(), Scanline, scanline);
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
    private static bool IsSupportedAI(string key)
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
    private bool PopulateProperty(string ai, string data, ref ShippingItem item)
    {
        var success = false;
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
                //var lotCode = data;
                success = true;
            }
            //ProductionDate data (YYMMDD)
            else if (ai.Equals("11"))
            {
                item.PackDate = DateTime.ParseExact(data, "yyMMdd", null);
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
                var netWt = float.Parse(data);
                //The fourth digit of this AI indicates the number of decimal places (see GS1 General Specifications for details).
                double power = float.Parse(ai.Substring(ai.Length - 2));
                var multiplier = (float)Math.Pow(10, power);
                netWt /= multiplier;
                item.PickWeight = netWt;
                success = true;
            }
        }
        return success;
    }

    /// <summary>
    /// Checks the leading digits of the given scanline to determine the next AI
    /// </summary>
    /// <param name="scanline"></param>
    /// <returns>the AI or Null if no AI was found</returns>
    private static string? GetNextAI(string scanline)
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
            return string.Empty;
        }
    }

    /// <summary>
    /// Searches the barcode's scanline for the given AI and returns its start index
    /// </summary>
    /// <param name="barcode">the barcode to search</param>
    /// <param name="ai">the AI to find the index of</param>
    /// <returns>The start index of the given AI or -1 if the ai was not found</returns>
    private static int FindAIStartIndex(GS1_128Barcode barcode, string ai)
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
    #endregion Methods
}
