using System.Drawing;
using System.Drawing.Printing;
using PdfiumViewer;

namespace BlOrders2023.Services;
internal class PDFPrinterService
{
    private readonly string pdfFilePath;
    private int CurrentPageIndex = 0;

    public PDFPrinterService(string pdfFilePath)
    {
        this.pdfFilePath = pdfFilePath;
    }

    public async Task PrintPdfAsync(PrinterSettings? settings = null)
    {
        await Task.Run( () => {
        using PrintDocument printDoc = new();
        printDoc.PrinterSettings = settings ?? new();
        printDoc.PrintPage += On_PrintPage;
        printDoc.DefaultPageSettings.Margins = new Margins(0,0,0,0);
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
        using PdfDocument doc = PdfDocument.Load(pdfFilePath);
        var image = doc.Render(CurrentPageIndex,(int)(doc.PageSizes[CurrentPageIndex].Width * 4f), (int)(doc.PageSizes[CurrentPageIndex].Height * 4f), 1200, 1200, false);
        image.Save(pdfFilePath + "image.bmp");
        e.Graphics?.DrawImage(image, e.PageBounds);
        e.HasMorePages = CurrentPageIndex < doc.PageCount - 1;
        CurrentPageIndex++;
    }
}
