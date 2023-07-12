using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Exceptions;
using ServiceStack.DataAnnotations;

namespace BlOrders2023.Models
{
    public class Code128Barcode : IBarcode
    {

        private int _productId;
        private float _weight;
        private DateTime _packDate;
        private string _serial;

        public Code128Barcode(ShippingItem item) : base(item)
        {
            throw new NotImplementedException();
        }

        public Code128Barcode(string scanline) : base(scanline)
        {
            Scanline = scanline.Trim();
            ParseScanline();
        }

        private void ParseScanline()
        {
            switch (Scanline.Length)
            {

                //B&L Parts 605375 041 1255 23193 424233 (company, product, weight, julian, serial)
                case 24:
                    {
                        _productId = int.Parse(Scanline.Substring(6, 3));
                        _weight = float.Parse(Scanline.Substring(9, 4)) / 100f;
                        var year = (DateTime.Today.Year - (DateTime.Today.Year % 100)) + int.Parse(Scanline.Substring(13, 2));
                        DateTimeOffset date = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                        //subtract 1 day from Jdate to account for starting at Jan 1
                        date = date.AddDays(int.Parse(Scanline.Substring(15, 3)) - 1);
                        _packDate = date.Date;
                        _serial = Scanline.Substring(18, 6);
                        break;
                    }
                case 26:
                    {
                        break;
                    }
                //    6053758120012552319300424233
                case 28:
                    {
                        break;
                    }
                default:
                    {
                        throw new InvalidBarcodeExcption($"Unsupported Barcode Length {Scanline.Length}", scanline: Scanline);
                    }
            }
        }

        public override string Scanline { get; }


        public override bool PopuplateProperties(ref ShippingItem item)
        {
            item.ProductID = _productId;
            item.PickWeight = _weight;
            item.PackDate = _packDate;
            item.PackageSerialNumber = _serial;
            return true;
        }
    }
}
