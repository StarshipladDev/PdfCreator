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
            
            PdfWriterFunction pdfWriter = new PdfWriterFunction();
            Assert.IsTrue(System.IO.Directory.Exists("TestImages"));
            Assert.IsTrue(System.IO.File.Exists("TestImages/TestImage.png"));

        }
        [TestMethod]
        public void TestIfFilesRead()
        {
            PdfWriterFunction pdfWriter = new PdfWriterFunction();
            string text ="#Title_Page(TitleImage)#";
            
            if (File.Exists(String.Format("TestScript.txt")))
            {
                File.Delete(String.Format("TestScript.txt"));
            }
            File.WriteAllLines("TestScript.txt",new string[]{ text });
            Assert.IsTrue(pdfWriter.ParseRenderFile("FMMF_2.txt"));
        }
    }
}
