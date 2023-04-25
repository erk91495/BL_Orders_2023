using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Reporting.ReportClasses
{
    internal class WholesaleInvoice : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
                    {
                        page.Margin(50);

                        page.Header().Height(100).Background(Colors.Grey.Lighten1);
                        page.Content().Background(Colors.Grey.Lighten3);
                        page.Footer().Height(50).Background(Colors.Grey.Lighten1);
                    });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    }
}
