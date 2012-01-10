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
        public void QuickTest()
        {
            int test = 1;
            Assert.AreEqual(1, test);
        }
    }
}
