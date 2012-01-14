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
    }
}
