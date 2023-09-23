using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Exceptions;
using ServiceStack;
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
            if(item == null || item.PickWeight == null || item.PackDate == null || item.PackageSerialNumber == null)
            {
                throw new ArgumentException();
            }
            _productId = item.ProductID;
            _weight = (float)item.PickWeight;
            _packDate = (DateTime)item.PackDate;
            _serial = item.PackageSerialNumber;
            Scanline = SynthesizeScanline(item.Scanline);
        }

        public Code128Barcode(string scanline) : base(scanline)
        {
            Scanline = scanline.Trim();
            ParseScanline();
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
                        //Go to the end to pick up the '_' for manually entered barcodes
                        _serial = Scanline[18..];
                        break;
                    }
                //B&L Ladder Barcode 605375 610 1255 23193 00424233
                case 26:
                    {
                        _productId = int.Parse(Scanline.Substring(6, 3));
                        _weight = float.Parse(Scanline.Substring(9, 4)) / 100f;
                        var year = (DateTime.Today.Year - (DateTime.Today.Year % 100)) + int.Parse(Scanline.Substring(13, 2));
                        DateTimeOffset date = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                        //subtract 1 day from Jdate to account for starting at Jan 1
                        date = date.AddDays(int.Parse(Scanline.Substring(15, 3)) - 1);
                        _packDate = date.Date;
                        //Go to the end to pick up the '_' for manually entered barcodes
                        _serial = Scanline[18..];
                        break;
                    }
                //DLM Turkey & Breast Barcodes 605375 812 001255 23193 00424233
                case 28:
                    {
                        _productId = int.Parse(Scanline.Substring(6, 3));
                        _weight = float.Parse(Scanline.Substring(9, 6)) / 100f;
                        var year = (DateTime.Today.Year - (DateTime.Today.Year % 100)) + int.Parse(Scanline.Substring(15, 2));
                        DateTimeOffset date = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                        //subtract 1 day from Jdate to account for starting at Jan 1
                        date = date.AddDays(int.Parse(Scanline.Substring(17, 3)) - 1);
                        _packDate = date.Date;
                        //Go to the end to pick up the '_' for manually entered barcodes
                        _serial = Scanline[20..];
                        break;
                    }
                default:
                    {
                        throw new InvalidBarcodeExcption($"Unsupported Barcode Length {Scanline.Length}", scanline: Scanline);
                    }
            }
        }

        private string SynthesizeScanline(string oldScanline)
        {
            string scanline;
            switch (oldScanline.Length)
            {

                //B&L Parts 605375 041 1255 23193 424233 (company, product, weight, julian, serial)
                case 24:
                    {
                        //_weight = float.Parse(Scanline.Substring(9, 4)) / 100f;
                        var weightString = Math.Round(_weight * 100f, 0).ToString().PadLeft(4, '0');
                        var startHalf = oldScanline[..9];
                        var endHalf = oldScanline[13..];
                        scanline = string.Concat(startHalf, weightString, endHalf);
                        break;
                    }
                //B&L Ladder Barcode 605375 610 1255 23193 00424233
                case 26:
                    {
                        //_weight = float.Parse(Scanline.Substring(9, 4)) / 100f;
                        var weightString = Math.Round(_weight * 100f, 0).ToString().PadLeft(4,'0');
                        var startHalf = oldScanline[..9];
                        var endHalf = oldScanline[13..];
                        scanline = string.Concat(startHalf, weightString, endHalf);
                        break;
                    }
                //DLM Turkey & Breast Barcodes 605375 812 001255 23193 00424233
                case 28:
                    {
                        //_weight = float.Parse(Scanline.Substring(9, 6)) / 100f;
                        var weightString = Math.Round(_weight * 100f, 0).ToString().PadLeft(6, '0');
                        var startHalf = oldScanline[..9];
                        var endHalf = oldScanline[15..];
                        scanline = string.Concat(startHalf, weightString, endHalf);
                        break;
                    }
                default:
                    {
                        throw new InvalidBarcodeExcption($"Unsupported Barcode Length {Scanline.Length}", scanline: Scanline);
                    }
            }
            return scanline;
        }

    }
}
