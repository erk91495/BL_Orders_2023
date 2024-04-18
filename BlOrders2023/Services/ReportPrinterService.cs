using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Reporting.ReportClasses;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Services;
internal class ReportPrinterService
{
    private readonly IEnumerable<byte[]> images;
    private int CurrentPageIndex = 0;

    internal ReportPrinterService(IReport report)
    {
        images = report.GenerateImages();
    }
    internal ReportPrinterService(IEnumerable<IReport> reports)
    {
        images = Document.Merge(reports).UseContinuousPageNumbers().GenerateImages();
    }


    public async Task PrintAsync(PrinterSettings? settings = null)
    {
        await Task.Run(() => {
            using PrintDocument printDoc = new();
            printDoc.PrinterSettings = settings ?? new();
            printDoc.PrintPage += On_PrintPage;
            printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            printDoc.OriginAtMargins = true;
            printDoc.Print();
        });
    }

    void On_BeginPrint(object sender, PrintEventArgs e)
    {
        CurrentPageIndex = 0;
    }


    private void On_PrintPage(object sender, PrintPageEventArgs e)
    {
        Bitmap image;
        using (var ms = new MemoryStream(images.ElementAt(CurrentPageIndex)))
        {
            image = new Bitmap(ms);
        }
        e.Graphics?.DrawImage(image, e.PageBounds);
        e.HasMorePages = CurrentPageIndex < images.Count() - 1;
        CurrentPageIndex++;
    }
}
