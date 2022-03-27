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

namespace PDFWriter
{
    public class PdfWriterFunction
    {

        Graphics g = null;
        Random rand = null;
        MemoryStream pdfFile = null;

        public enum commandEnums{
            TitlePage,
            None
        }
        public PdfWriterFunction()
        {
            LoadImage();
            CreateTestPdf();
        }

        public void CreateTestPdf()
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
        public void WriteBasicText()
        {

        }
        public void LoadImage()
        {
            Bitmap testImage = new Bitmap(210, 297);
            g = Graphics.FromImage(testImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            filltestImage(testImage);

        }
        public void filltestImage(Bitmap img)
        {
            rand = new Random();
            for( int x =0 ;x< img.Width; x++)
            {

                int r = rand.Next(0, 255);
                int g = rand.Next(0, 255);
                int b = rand.Next(0, 255);
                for (int y = 0; y < img.Height; y++)
                {
                    r= (int)Math.Abs(r+ rand.Next(10)-5)%255;
                    g = (int)Math.Abs(g + rand.Next(10) - 5) % 255;
                    b = (int)Math.Abs(b + rand.Next(10) - 5)%255;
                    img.SetPixel(x,y, System.Drawing.Color.FromArgb(255,r,g,b));
                }
            }
            var brush = new SolidBrush(System.Drawing.Color.Black);
            var font = new Font("Times New Roman", 12.0f);
             font = new Font(font, FontStyle.Bold);
            g.DrawString("Test PDF File", font, brush, img.Width / 2, font.Size);
            if( !System.IO.Directory.Exists("TestImages"))
            {
                System.IO.Directory.CreateDirectory("TestImages");
            }
            img.Save("TestImages/TestImage.png");
        }
        public commandEnums ReadRenderFileLine(string line)
        {
            bool hasCommand = (line != null && line.Length > 0 && line.ToCharArray()[0] == '#' && line.Trim(' ').ToCharArray()[line.Trim(' ').Length-1]=='#');
            if (!hasCommand)
            {
                return commandEnums.None;
            }
            else
            {
                var commandTest = "title_page".ToLower();
                if (line.Length>commandTest.Length+2 && line.ToLower().Substring(1, commandTest.Length).Equals(commandTest))
                {
                    return commandEnums.TitlePage;
                }
                return commandEnums.None;
            }
        }
        public bool ParseRenderFile(string fileName)
        {
            var streamReaderLines = File.ReadAllLines(fileName);
            foreach( var line in streamReaderLines)
            {
                var result = ReadRenderFileLine(line);
                if (result != commandEnums.None)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
