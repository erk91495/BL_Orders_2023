using System.Drawing;
using System.Drawing.Printing;
using Syncfusion.PdfToImageConverter;

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
        PdfToImageConverter Converter = new();
        FileStream inputStream = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read);
        Converter.Load(inputStream);
        //SizeF size = new(850, 1100);
        Stream outputStream = Converter.Convert(CurrentPageIndex, false, false);
        Image image = Image.FromStream(outputStream);
        e.Graphics?.DrawImage(image, e.MarginBounds);

        e.HasMorePages = CurrentPageIndex < Converter.PageCount - 1;
        CurrentPageIndex++;
    }
}
