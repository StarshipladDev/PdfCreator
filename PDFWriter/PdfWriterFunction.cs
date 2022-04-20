using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PDFWriter.TestFunctions;
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
        Document pdfDocument = null;
        public enum commandEnums{
            TitlePage,
            HeaderImage,
            TextBoxStart,
            TextBoxEnd,
            HeaderText,
            None
        }
        public PdfWriterFunction(bool isTest = false)
        {
            if (isTest)
            {
                LoadImage();
                PDFCreatorTestFunctions.CreateTestPdf();
            }
        }
        public void CreatePDF(string FileName)
        {
            if (!System.IO.Directory.Exists("TestPdfs"))
            {
                System.IO.Directory.CreateDirectory("TestPdfs");
            }
            if (File.Exists(String.Format("TestPdfs/{0}.pdf", FileName)))
            {
                File.Delete(String.Format("TestPdfs/{0}.pdf", FileName));
            }
            string destination = "TestPdfs/pdf.pdf";
            PdfWriter pdfWriter = new PdfWriter(destination);
            PdfDocument pdfDoc = new PdfDocument(pdfWriter);
            Document document = new Document(pdfDoc);
            document.SetMargins(0f, 0f, 0f, 0f);
        }
        public void WriteBasicText(string lineOfText)
        {



            /* Title Page */
            ImageData imgFile = ImageDataFactory.Create(@"TestImages/TestImage.png");
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(imgFile)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetHeight(100)
                .SetAutoScale(true);
            this.pdfDocument.Add(img);


            /*New Page */
            this.pdfDocument.Add(new AreaBreak());
            /* First Page Content */
            Paragraph para = new Paragraph(lineOfText)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(14);
            this.pdfDocument.Add(para);
        }
        public void WriteHeaderText(string lineOfText)
        {
            /*New Page */
            this.pdfDocument.Add(new AreaBreak());

            LineSeparator seperator = new LineSeparator(new SolidLine());
            this.pdfDocument.Add(seperator);
            /* CreatePDF Bold Font */
            Font header = new Font(FontFamily.Families.First(),20,FontStyle.Bold);
            /* First Page Content */
            Paragraph para = new Paragraph(lineOfText)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(20);
            this.pdfDocument.Add(para);
        }
        public void LoadImage()
        {
            Bitmap testImage = new Bitmap(210, 297);
            g = Graphics.FromImage(testImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            FilltestImage(testImage);

        }
        public void FilltestImage(Bitmap img)
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
        public commandEnums GetFileLineCommandType(string line)
        {
            line = line.Trim(' ');
            bool hasCommand = (line != null && line.Length > 0 && line.ToCharArray()[0] == '#' && line.ToCharArray()[line.Length-1]=='#');
            if (!hasCommand)
            {
                return commandEnums.None;
            }
            else
            {

                var commandArray = Enum.GetNames(commandEnums.None.GetType());
                foreach (var command in commandArray)
                {
                    if (line.Length > command.Length + 2 && line.ToLower().Substring(1, command.Length).Equals(command))
                    {
                        if (commandArray.ToList().IndexOf(command) != -1) {
                            return (commandEnums)Enum.Parse(commandEnums.None.GetType(), command);
                        } 
                    }
                }
                return commandEnums.None;
            }
        }
        public bool ParseRenderFile(string fileName)
        {
            bool isTextBox = false;
            var streamReaderLines = File.ReadAllLines(fileName);
            foreach( var line in streamReaderLines)
            {
                try
                {
                    var result = GetFileLineCommandType(line);
                    if (result != commandEnums.None)
                    {

                        if (result == commandEnums.TextBoxStart && !isTextBox)
                        {
                            isTextBox = true;
                        }

                        else if (result == commandEnums.HeaderText)
                        {
                            WriteHeaderText(line.Trim(' ').Substring(line.IndexOf('(')+1, line.LastIndexOf(')')-line.IndexOf('(') + 1));
                        }
                    }
                    else
                    {
                        if (isTextBox)
                        {
                            WriteBasicText(line);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error encountered: "+e.StackTrace+" \n "+e.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
