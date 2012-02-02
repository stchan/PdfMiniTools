using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using iTextSharpText = iTextSharp.text;
using iTextSharpPDF = iTextSharp.text.pdf;

using NUnit.Framework;

namespace PdfMiniToolsTests
{
    [TestFixture]
    public class Tests
    {
        #region Ctor
        public Tests()
        {}
        #endregion

        [Test]
        public void TestConcatenatePDFFiles()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            String[] inputFiles = new String[] { @"..\..\Heart_of_Darkness_NT.pdf", @"..\..\Beginning GIMP.pdf" };
            String outputFile = @"..\..\TESTCONCAT.pdf";
            coreTest.ConcatenatePDFFiles(inputFiles, outputFile);
        }

        [Test]
        public void TestExtractPages_GoldenPath()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            coreTest.ExtractPDFPages(@"..\..\Heart_of_Darkness_NT.pdf", @"..\..\Heart_of_Darkness_66_101.pdf", 66, 101);
            Assert.IsTrue(ArePagesIdentical(@"..\..\Heart_of_Darkness_66_101.pdf", 1, 36, @"..\..\Heart_of_Darkness_NT.pdf", 66));
        }

        private bool ArePagesIdentical(String firstPdf, int firstStartPage, int firstLastPage,
                                       String secondPdf, int secondStartPage)
        {
            bool pagesAreIdentical = true;
            var firstPdfReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(firstPdf), null);
            var secondPdfReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(secondPdf), null);


            int secondPdfPage = secondStartPage;
            try
            {
                for (int currentFirstPage = firstStartPage; currentFirstPage < firstLastPage; currentFirstPage++)
                {
                    if (BitConverter.ToInt32(new MD5CryptoServiceProvider().ComputeHash(firstPdfReader.GetPageContent(currentFirstPage)), 0)
                        != BitConverter.ToInt32(new MD5CryptoServiceProvider().ComputeHash(secondPdfReader.GetPageContent(secondPdfPage)), 0))
                    {
                        pagesAreIdentical = false;
                        break;
                    }
                    secondPdfPage++;
                }
            }
            finally
            {
                if (firstPdfReader != null) firstPdfReader.Close();
                if (secondPdfReader != null) secondPdfReader.Close();
            }
            return pagesAreIdentical;
        }

        [Test]
        public void TestParsePDFDateTime()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            // Valid pdf datetime string
            Assert.IsNotNull(coreTest.TryParsePDFDateTime("D:20020920162615+10'00'"));
            // Valid pdf datetime string
            Assert.IsNotNull(coreTest.TryParsePDFDateTime("D:19991120202635-10'00'"));
            // Too short
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920162615+10"));
            // Too long
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920162615+10'00'59"));
            // Invalid month
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20021920162615+10'00'"));
            // Invalid day
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020900162615+10'00'"));
            // Invalid hour
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920292615+10'00'"));
            // Invalid minute
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920166015+10'00'"));
            // Invalid second
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920162678+10'00'"));
            // Invalid timezone indicator
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920162615U10'00'"));
            // Invalid offset hours
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920162615+29'00'"));
            // Invalid offset minutes
            Assert.IsNull(coreTest.TryParsePDFDateTime("D:20020920162615+10'63'"));
            // Null argument
            Assert.Throws<ArgumentNullException>(delegate { coreTest.TryParsePDFDateTime(null); });
        }

        [Test]
        public void TestRetrieveBasicProperties()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            Dictionary<String, String> basicPropertiesDictionary = coreTest.RetrieveBasicProperties(@"..\..\Heart_of_Darkness_NT.pdf");
            Assert.IsTrue(basicPropertiesDictionary.Count == 4);
            Assert.IsTrue(basicPropertiesDictionary.ContainsKey("Page Count"));
            Assert.IsTrue(basicPropertiesDictionary.ContainsKey("Encrypted"));
            Assert.IsTrue(basicPropertiesDictionary.ContainsKey("Pdf Version"));
            Assert.IsTrue(basicPropertiesDictionary.ContainsKey("Rebuilt"));

        }

        [Test]
        public void TestRetrieveInfo()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            Dictionary<String, String> pdfInfo = coreTest.RetrieveInfo(@"..\..\Heart_of_Darkness_NT.pdf");
            Assert.IsTrue(pdfInfo.Count > 0);
        }

        [Test]
        public void TestSplitPDF()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            String testFile = @"..\..\Heart_of_Darkness_NT.pdf";
            var pageSplits = new SortedList<int, String>();
            pageSplits.Add(1, @"..\..\Heart_of_Darkness_01.pdf");
            pageSplits.Add(11, @"..\..\Heart_of_Darkness_02.pdf");
            pageSplits.Add(86, @"..\..\Heart_of_Darkness_03.pdf");
            pageSplits.Add(111, @"..\..\Heart_of_Darkness_04.pdf");
            coreTest.SplitPDF(testFile, pageSplits);
        }

    }
}
