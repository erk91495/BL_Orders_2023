using System.Drawing;
using System.Drawing.Printing;
using Syncfusion.EJ2.PdfViewer;

namespace BlOrders2023.Services;
internal class PDFPrinterService
{
    private string pdfFilePath;
    private int CurrentPageIndex = 0;

    public PDFPrinterService(string pdfFilePath)
    {
        this.pdfFilePath = pdfFilePath;
    }

    public void PrintPdf(PrinterSettings? settings = null)
    {
        using PrintDocument printDoc = new PrintDocument();
        printDoc.PrinterSettings = settings ?? new();
        printDoc.PrintPage += On_PrintPage;
        printDoc.DefaultPageSettings.Margins = new Margins(0,0,0,0);
        printDoc.OriginAtMargins = true;
        printDoc.Print();
    }

    void On_BeginPrint(object sender, PrintEventArgs e)
    {
        CurrentPageIndex = 0;
    }

    private void On_PrintPage(object sender, PrintPageEventArgs e)
    {
        PdfRenderer toPrint = new();
        toPrint.Load(pdfFilePath);
        SizeF size = new(850, 1100);
        var bitmap = toPrint.ExportAsImage(CurrentPageIndex, 600, 600);
        //bitmap.Save(Path.GetTempPath() + "BLOrders2023\\" + "temp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        e.Graphics.DrawImage(bitmap, e.MarginBounds);
        
        e.HasMorePages = CurrentPageIndex < toPrint.PageCount - 1;
        CurrentPageIndex++;
    }
}
