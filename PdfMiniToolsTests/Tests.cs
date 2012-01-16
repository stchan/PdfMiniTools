using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public void TestParsePDFDateTime()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            // Valid pdf datetime string
            Assert.IsNotNull(coreTest.ParsePDFDateTime("D:20020920162615+10'00'"));
            // Valid pdf datetime string
            Assert.IsNotNull(coreTest.ParsePDFDateTime("D:19991120202635-10'00'"));
            // Too short
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920162615+10"));
            // Too long
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920162615+10'00'59"));
            // Invalid month
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20021920162615+10'00'"));
            // Invalid day
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020900162615+10'00'"));
            // Invalid hour
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920292615+10'00'"));
            // Invalid minute
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920166015+10'00'"));
            // Invalid second
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920162678+10'00'"));
            // Invalid timezone indicator
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920162615U10'00'"));
            // Invalid offset hours
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920162615+29'00'"));
            // Invalid offset minutes
            Assert.IsNull(coreTest.ParsePDFDateTime("D:20020920162615+10'63'"));
            // Null argument
            Assert.Throws<ArgumentNullException>(delegate { coreTest.ParsePDFDateTime(null); });
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
        public void TestRetrieveCatalog()
        {
            PdfMiniToolsCore.CoreTools coreTest = new PdfMiniToolsCore.CoreTools();
            Dictionary<String, String> pdfCatalog = coreTest.RetrieveCatalog(@"..\..\Heart_of_Darkness_NT.pdf");
            Assert.IsTrue(pdfCatalog.Count > 0);
        }

    }
}
