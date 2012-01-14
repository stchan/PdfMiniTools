using System;
using System.Collections.Generic;
using System.Text;

using iTextSharpText = iTextSharp.text;
using iTextSharpPDF = iTextSharp.text.pdf;

namespace PdfMiniToolsCore
{
    
    public class CoreTools
    {

        #region Ctor

        #endregion
        #region Constants

        private const String exceptionArgumentNullOrEmptyString = "Parameter cannot be null, an empty string, or all whitespace.";

        #endregion

        public Dictionary<String, String> RetrieveBasicProperties(String filename)
        {
            Dictionary<String, String> basicProperties = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(filename) && !String.IsNullOrWhiteSpace(filename))
            {
                iTextSharpPDF.PdfReader documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
                basicProperties.Add("Page Count", documentReader.NumberOfPages.ToString());
                basicProperties.Add("Encrypted", documentReader.IsEncrypted().ToString());
                basicProperties.Add("Pdf Version", documentReader.PdfVersion.ToString());
                basicProperties.Add("Rebuilt", documentReader.IsRebuilt().ToString());
                documentReader.Close();
            }
            else
            {
                throw new ArgumentNullException("filename", exceptionArgumentNullOrEmptyString);
            }
            return basicProperties;
        }

        public Dictionary<String, String> RetrieveInfo(String filename)
        {
            Dictionary<String, String> pdfInfo = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(filename) && !String.IsNullOrWhiteSpace(filename))
            {
                iTextSharpPDF.PdfReader documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
                foreach (object infoKey in documentReader.Info.Keys)
                {
                    if ((infoKey as String) != null)
                    {
                        pdfInfo.Add(infoKey as String, documentReader.Info[infoKey as String] as String);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("filename", exceptionArgumentNullOrEmptyString);
            }
            return pdfInfo;
        }
    }
}
