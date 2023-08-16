using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models.Helpers;
public static class BarcodeHelpers
{

    /// <summary>
    /// This method caluculates and appends the check digit for the given gtin in accordance with the gs1 standard
    /// see 
    /// </summary>
    /// <param name="gtin">The gtin to calgulate</param>
    /// <returns> True if the calculation is successful</returns>
    public static bool AppendG10CheckDigit(ref string gtin)
    {
        var success = false;

        if (gtin.Length <= 13)
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
    public static string CalculateG10CheckDigit(in string gtin)
    {
        var sum = 0;
        //SSCC cheksum is 18 digits
        var j = 0;
        for (var i = gtin.Length - 1; i >= 0; i--)
        {
            var c = gtin[i];
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
        var checkDigit = 10 - (sum % 10);
        return checkDigit.ToString();
    }
}
