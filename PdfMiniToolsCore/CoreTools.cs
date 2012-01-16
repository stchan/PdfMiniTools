using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

        #region PDF Information methods
        /// <summary>
        /// Converts a PDF date format string to local time, as a DateTimeOffset.
        /// Matching is strict - all fields are required, unlike in the official
        /// definition in which everything after the year is optional.
        /// Expected format is:
        /// D:YYYYMMDDHHmmSSOHH'mm'
        /// 
        /// The D: prefix, and single quotes are expected literals.
        /// O is the timezone indicator, and can be +,-, or Z.
        /// </summary>
        /// <param name="pdfDateTime">The PDF date string</param>
        /// <returns>
        /// A DateTimeOffSet? object containing the converted date/time/offset (local time), or
        /// null if the conversion was not possible or failed.
        /// </returns>
        public DateTimeOffset? ParsePDFDateTime(String pdfDateTime)
        {
            
            DateTimeOffset? parsedDate = null;
            if (!String.IsNullOrEmpty(pdfDateTime) && !String.IsNullOrWhiteSpace(pdfDateTime))
            {
                Regex pdfDateTimeRegex = new Regex(@"^D:[0-9]{4}[0,1]{1}[0-9]{1}[0-3]{1}[0-9]{1}[0-2]{1}[0-9]{1}[0-5]{1}[0-9]{1}[0-5]{1}[0-9]{1}[\+\-Z]{1}[0-2]{1}[0-9]{1}'[0-5]{1}[0-9]{1}'$");
                if (pdfDateTimeRegex.IsMatch(pdfDateTime))
                {
                    int pdfYear = Convert.ToInt32(pdfDateTime.Substring(2, 4));
                    int pdfMonth = Convert.ToInt32(pdfDateTime.Substring(6, 2));
                    int pdfDay = Convert.ToInt32(pdfDateTime.Substring(8, 2));
                    int pdfHour = Convert.ToInt32(pdfDateTime.Substring(10, 2));
                    int pdfMinute = Convert.ToInt32(pdfDateTime.Substring(12, 2));
                    int pdfSecond = Convert.ToInt32(pdfDateTime.Substring(14, 2));
                    TimeSpan pdfDateOffset = TimeSpan.Zero;
                    try
                    {
                        switch (pdfDateTime.Substring(16, 1))
                        {
                            case "Z":
                                break;
                            default:
                                int pdfDateOffsetHour = Convert.ToInt32(pdfDateTime.Substring(17, 2));
                                int pdfDateOffsetMinute = Convert.ToInt32(pdfDateTime.Substring(20, 2));
                                pdfDateOffset = new TimeSpan(pdfDateOffsetHour, pdfDateOffsetMinute, 0);
                                if (pdfDateTime.Substring(16, 1) == "-") pdfDateOffset = pdfDateOffset.Negate();
                                break;
                        }
                        parsedDate = new DateTimeOffset(pdfYear, pdfMonth, pdfDay, pdfHour, pdfMinute, pdfSecond, pdfDateOffset);
                    }
                    catch (ArgumentOutOfRangeException)
                    { }
                    catch (ArgumentException)
                    { }
                    if (parsedDate != null) parsedDate = ((DateTimeOffset)parsedDate).ToLocalTime();
                }
            }
            else
            {
                throw new ArgumentNullException(pdfDateTime, exceptionArgumentNullOrEmptyString);
            }
            return parsedDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Dictionary<String, String> RetrieveCatalog(String filename)
        {
            Dictionary<String, String> pdfCatalog = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(filename) && !String.IsNullOrWhiteSpace(filename))
            {
                iTextSharpPDF.PdfReader documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
                IDictionaryEnumerator pdfCatalogEnumerator = documentReader.Catalog.GetEnumerator();
                while (pdfCatalogEnumerator.MoveNext())
                {
                    String pdfCatalogValue = null;
                    DateTimeOffset? pdfCatalogDate;
                    if (pdfCatalogEnumerator.Value != null)
                    {
                        pdfCatalogDate = ParsePDFDateTime(pdfCatalogEnumerator.Value.ToString());
                        if (pdfCatalogDate.HasValue)
                        {
                            pdfCatalogValue = String.Format("{0:F}", ((DateTimeOffset)pdfCatalogDate).DateTime);
                        }
                        else
                        {
                            pdfCatalogValue = pdfCatalogEnumerator.Value.ToString();
                        }
                    }
                    pdfCatalog.Add(pdfCatalogEnumerator.Key as String, pdfCatalogValue);
                }
            }
            else
            {
                throw new ArgumentNullException("filename", exceptionArgumentNullOrEmptyString);
            }
            return pdfCatalog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Dictionary<String, String> RetrieveInfo(String filename)
        {
            Dictionary<String, String> pdfInfo = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(filename) && !String.IsNullOrWhiteSpace(filename))
            {
                iTextSharpPDF.PdfReader documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
                IDictionaryEnumerator pdfInfoEnumerator = documentReader.Info.GetEnumerator();
                while (pdfInfoEnumerator.MoveNext())
                {
                    String pdfInfoValue = null;
                    DateTimeOffset? pdfInfoDate;
                    if (pdfInfoEnumerator.Value != null)
                    {
                        pdfInfoDate = ParsePDFDateTime(pdfInfoEnumerator.Value.ToString());
                        if (pdfInfoDate.HasValue)
                        {
                            pdfInfoValue = String.Format("{0:F}", ((DateTimeOffset)pdfInfoDate).DateTime);
                        }
                        else
                        {
                            pdfInfoValue = pdfInfoEnumerator.Value.ToString();
                        }
                    }
                    pdfInfo.Add(pdfInfoEnumerator.Key as String, pdfInfoValue);
                }
            }
            else
            {
                throw new ArgumentNullException("filename", exceptionArgumentNullOrEmptyString);
            }
            return pdfInfo;
        }

        #endregion

        #region PDF File operation methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFiles">A string array containing the names of the pdf files to concatenate</param>
        /// <param name="outputFile">Name of the concatenated file.</param>
        public void ConcatenatePDFFiles(String[] inputFiles, String outputFile)
        {
            if (inputFiles != null && inputFiles.Length > 0)
            {
                if (!String.IsNullOrEmpty(outputFile) && !String.IsNullOrWhiteSpace(outputFile))
                {
                    iTextSharpText.Document concatDocument = new iTextSharpText.Document();
                    iTextSharpPDF.PdfCopy outputCopy = new iTextSharpPDF.PdfCopy(concatDocument, new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite));
                    concatDocument.Open();
                    try
                    {
                        for (int loop = 0; loop <= inputFiles.GetUpperBound(0); loop++)
                        {
                            iTextSharpPDF.PdfReader inputDocument = new iTextSharpPDF.PdfReader(inputFiles[loop]);
                            for (int pageLoop = 1; pageLoop <= inputDocument.NumberOfPages; pageLoop++)
                            {
                                concatDocument.SetPageSize(inputDocument.GetPageSizeWithRotation(pageLoop));
                                outputCopy.AddPage(outputCopy.GetImportedPage(inputDocument, pageLoop));
                            }
                            inputDocument.Close();
                            outputCopy.FreeReader(inputDocument);
                            inputDocument = null;
                        }
                        concatDocument.Close();
                        outputCopy.Close();
                    }
                    catch
                    {
                        if (concatDocument != null && concatDocument.IsOpen()) concatDocument.Close();
                        if (outputCopy != null) outputCopy.Close();
                        if (File.Exists(outputFile))
                        {
                            try
                            {
                                File.Delete(outputFile);
                            }
                            catch { }
                        }
                        throw;
                    }
                }
                else
                {
                    throw new ArgumentNullException("outputFile", exceptionArgumentNullOrEmptyString);
                }
            }
            else
            {
                throw new ArgumentNullException("inputFiles", exceptionArgumentNullOrEmptyString);
            }
        }

        #endregion
    }
}
