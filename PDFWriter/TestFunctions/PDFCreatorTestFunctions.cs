using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFWriter.TestFunctions
{
    internal static class PDFCreatorTestFunctions
    {

        public  static void CreateTestPdf()
        {

            if (!System.IO.Directory.Exists("TestPdfs"))
            {
                System.IO.Directory.CreateDirectory("TestPdfs");
            }
            if (File.Exists("TestPdfs/pdf.pdf"))
            {
                File.Delete("TestPdfs/pdf.pdf");
            }
            string destination = "TestPdfs/pdf.pdf";
            PdfWriter pdfWriter = new PdfWriter(destination);
            PdfDocument pdfDoc = new PdfDocument(pdfWriter);
            Document document = new Document(pdfDoc);
            document.SetMargins(0f, 0f, 0f, 0f);


            /* Title Page */
            ImageData imgFile = ImageDataFactory.Create(@"TestImages/TestImage.png");
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(imgFile)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetHeight(100)
                .SetAutoScale(true);
            document.Add(img);


            /*New Page */
            document.Add(new AreaBreak());
            /* First Page Content */
            Paragraph para = new Paragraph("TestHeader")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(16);
            document.Add(para);
            LineSeparator seperator = new LineSeparator(new SolidLine());
            document.Add(seperator);
            Table textTable = new Table(2, false);
            Cell textCell1 = new Cell(1, 1)
                .Add(new Paragraph("Here is some test text"));
            Cell textCell2 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.GRAY)
                .Add(new Paragraph("Here is some test text"));
            textTable.AddCell(textCell1);
            textTable.AddCell(textCell2);
            document.Add(textTable);
            document.Close();

        }
    }
}
