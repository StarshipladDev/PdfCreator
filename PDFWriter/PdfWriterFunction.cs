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
        public PdfWriterFunction(bool isTest = false,bool isTestOfProperFunctionality = false)
        {
            if (isTest)
            {
                PDFCreatorTestFunctions.LoadImage();
                PDFCreatorTestFunctions.CreateTestPdf();
            }
            else if (!isTestOfProperFunctionality)
            {
                Console.WriteLine("Select a File To Draw a PDF From :" );

                var listOfFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
                foreach (var file in listOfFiles)
                {
                    Console.WriteLine("File :" + file);
                }
                var fileName = Console.ReadLine();
                if (File.Exists(fileName)) {
                    Console.WriteLine("Creating File");
                    var pdfFileName = Path.GetFileNameWithoutExtension(fileName);
                    CreatePDF(pdfFileName);
                    ParseRenderFile(fileName);
                }
                else
                {
                    Console.WriteLine("Cannot locate "+fileName);
                }
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
            string destination = String.Format("TestPdfs/{0}.pdf", FileName);
            PdfWriter pdfWriter = new PdfWriter(destination);
            PdfDocument pdfDoc = new PdfDocument(pdfWriter);
            Document document = new Document(pdfDoc);
            document.SetMargins(0f, 0f, 0f, 0f);
            this.pdfDocument = document;
        }
        public void WriteBasicText(string lineOfText)
        {
            /* First Page Content */
            Paragraph para = new Paragraph(lineOfText)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(14);
            this.pdfDocument.Add(para);
        }
        public void WriteHeaderText(string lineOfText)
        {
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
        public bool InputImage(string imageFileName,int scale = 100)
        {
            if (File.Exists(imageFileName) && this.pdfDocument!= null)
            {

                ImageData imgFile = ImageDataFactory.Create(@imageFileName);
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(imgFile)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetHeight(scale)
                    .SetAutoScale(true);
                this.pdfDocument.Add(img);
                return true;
            }
            else
            {
                return false;
            }
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
                    if (line.Length >= command.Length + 2 && line.ToLower().Substring(1, command.Length).Equals(command.ToLower()))
                    {
                        if (commandArray.ToList().IndexOf(command) != -1) {
                            return (commandEnums)Enum.Parse(commandEnums.None.GetType(), command);
                        } 
                    }
                }
                return commandEnums.None;
            }
        }

        public string GetArgumentInBrackets(string commandLine)
        {
            if(commandLine.IndexOf('(')!= -1 && commandLine.LastIndexOf(')') != -1)
            {
                return commandLine.Trim(' ').Substring(commandLine.IndexOf('(') + 1, commandLine.LastIndexOf(')') - commandLine.IndexOf('(') - 1);
            }
            return commandLine;
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
                        else if (result == commandEnums.TextBoxEnd && isTextBox)
                        {
                            isTextBox = false;
                            WriteBasicText("\n");
                        }
                        else if (result == commandEnums.TitlePage)
                        {
                            InputImage(GetArgumentInBrackets(line));
                        }
                        else if (result == commandEnums.HeaderText)
                        {
                            WriteHeaderText(GetArgumentInBrackets(line));
                            WriteBasicText("\n");
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
            if (this.pdfDocument != null) 
            { 
                this.pdfDocument.Close(); 
            }
            return true;
        }
    }
}
