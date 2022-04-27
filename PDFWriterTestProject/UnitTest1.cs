using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFWriter;
using System;
using System.IO;

namespace PDFWriterTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            if (System.IO.Directory.Exists("TestImages")){
                if (System.IO.File.Exists("TestImages/TestImage.png")){
                    System.IO.File.Delete("TestImages/TestImage.png");
                }
                System.IO.Directory.Delete("TestImages");
            }
            
            PdfWriterFunction pdfWriter = new PdfWriterFunction(true);
            Assert.IsTrue(System.IO.Directory.Exists("TestImages"));
            Assert.IsTrue(System.IO.File.Exists("TestImages/TestImage.png"));

        }
        [TestMethod]
        public void TestIfFilesRead()
        {
            PdfWriterFunction pdfWriter = new PdfWriterFunction(false,true);
            string text = "#Title_Page(TestImages/TestImage.png)#";
            
            if (File.Exists(String.Format("TestScript.txt")))
            {
                File.Delete(String.Format("TestScript.txt"));
            }
            if (System.IO.Directory.Exists("TestPdfs"))
            {
                if (File.Exists(String.Format("TestPdfs/TestScript.pdf")))
                {
                    File.Delete(String.Format("TestPdfs/TestScript.pdf"));
                } 
            }
            File.WriteAllLines("TestScript.txt",new string[]{ text });
            pdfWriter.CreatePDF("TestScript");
            Assert.IsTrue(pdfWriter.ParseRenderFile("TestScript.txt"));
            Assert.IsTrue(File.Exists("TestPdfs/TestScript.pdf"));
        }
    }
}
