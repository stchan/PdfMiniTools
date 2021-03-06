﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using iTextSharpText = iTextSharp.text;
using iTextSharpPDF = iTextSharp.text.pdf;

using BouncyX509 = Org.BouncyCastle.X509;

namespace PdfMiniToolsCore
{
    
    public class CoreTools
    {

        #region Ctor

        #endregion
        #region Constants

        private const String exceptionArgumentNullOrEmptyString = "Parameter cannot be null, an empty string, or all whitespace.";
        private const String exceptionArgumentZeroOrNegative = "Parameter cannot be zero or negative.";
        private const String exceptionParameterCannotBeLessThan = "{0} cannot be less than {1}";
        private const String exceptionParameterCannotBeGreaterThan = "{0} cannot be great than {1}";

        #endregion

        #region PDF Information methods
        /// <summary>
        /// Converts a PDF date format string to local time, as a DateTimeOffset.
        /// </summary>
        /// <param name="pdfDateTime">The PDF date string</param>
        /// <returns>
        /// A DateTimeOffSet? object containing the converted date/time/offset (local time), or
        /// null if the conversion was not possible or failed.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>
        /// Matching is strict - all fields are required, unlike in the official
        /// definition in which everything after the year is optional.
        /// 
        /// Expected format is:
        /// D:YYYYMMDDHHmmSSOHH'mm'
        /// 
        /// The D: prefix, and single quotes are expected literals.
        /// O is the timezone indicator, and can be +,-, or Z.
        /// </remarks>
        public DateTimeOffset? TryParsePDFDateTime(String pdfDateTime)
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
        /// Checks if a file has a valid PDF structure by
        /// trying to create a PdfReader object for it.
        /// This function will only check for valid PDF header
        /// and xref information, it will not detect corrupt
        /// content within.
        /// </summary>
        /// <param name="filename">The PDF file to check</param>
        /// <returns>true if file's pdf structure is valid, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Will be thrown if <paramref name="filename"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Will be thrown if <paramref name="filename"/> cannot be found or accessed.</exception>
        public bool FileHasValidPDFStructure(String filename)
        {
            bool structureIsValid = false;
            if (!String.IsNullOrEmpty(filename) &&
                !String.IsNullOrWhiteSpace(filename) &&
                File.Exists(filename))
            {
                iTextSharpPDF.PdfReader inputDocument = null;
                try
                {
                    inputDocument = new iTextSharpPDF.PdfReader(filename);
                    structureIsValid = true;
                }
                catch (IOException) {  }
                finally
                {
                    if (inputDocument != null) inputDocument.Close();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(filename) ||
                    String.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(filename);
                else throw new FileNotFoundException();
            }
            return structureIsValid;
        }

        /// <summary>
        /// Retrieves the following properties as <see cref="Dictionary&lt;String,String&gt;"/> key/value pairs:
        /// Page Count, Encrypted, Pdf Version, Rebuilt
        /// </summary>
        /// <param name="filename">The PDF file</param>
        /// <returns>A <see cref="Dictionary&lt;String, String&gt;"/> object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks></remarks>
        public Dictionary<String, String> RetrieveBasicProperties(String filename)
        {
            Dictionary<String, String> basicProperties = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(filename) && !String.IsNullOrWhiteSpace(filename))
            {
                var documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
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
        public Dictionary<String, String> RetrieveAcroFieldsData(String filename)
        {
            Dictionary<String, String> fieldsData = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(filename) && !String.IsNullOrWhiteSpace(filename))
            {
                var documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
                IDictionaryEnumerator fieldsEnumerator = documentReader.AcroFields.Fields.GetEnumerator();
                ICollection fieldKeyValues = documentReader.AcroFields.Fields.Keys;
                foreach (String keyValue in fieldKeyValues)
                {
                    String pdfInfoValue = documentReader.AcroFields.GetField(keyValue.ToString());
                    DateTimeOffset? pdfInfoDate;
                    if (pdfInfoValue != null && !String.IsNullOrWhiteSpace(pdfInfoValue) &&
                                                          !String.IsNullOrEmpty(pdfInfoValue))
                    {
                        pdfInfoDate = TryParsePDFDateTime(pdfInfoValue);
                        if (pdfInfoDate.HasValue)
                        {
                            pdfInfoValue = String.Format("{0:F}", ((DateTimeOffset)pdfInfoDate).DateTime);
                        }
                    }
                    fieldsData.Add(keyValue, pdfInfoValue);

                }
            }
            return fieldsData;
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
                var documentReader = new iTextSharpPDF.PdfReader(new iTextSharpPDF.RandomAccessFileOrArray(filename), null);
                IDictionaryEnumerator pdfInfoEnumerator = documentReader.Info.GetEnumerator();
                while (pdfInfoEnumerator.MoveNext())
                {
                    String pdfInfoValue = null;
                    DateTimeOffset? pdfInfoDate;
                    if (pdfInfoEnumerator.Value != null && !String.IsNullOrWhiteSpace(pdfInfoEnumerator.Value.ToString()) &&
                                                           !String.IsNullOrEmpty(pdfInfoEnumerator.Value.ToString()))
                    {
                        pdfInfoDate = TryParsePDFDateTime(pdfInfoEnumerator.Value.ToString());
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
        /// Concatenates two or more PDF files into one file.
        /// </summary>
        /// <param name="inputFiles">A string array containing the names of the pdf files to concatenate</param>
        /// <param name="outputFile">Name of the concatenated file.</param>
        public void ConcatenatePDFFiles(String[] inputFiles, String outputFile)
        {
            if (inputFiles != null && inputFiles.Length > 0)
            {
                if (!String.IsNullOrEmpty(outputFile) && !String.IsNullOrWhiteSpace(outputFile))
                {
                    var concatDocument = new iTextSharpText.Document();
                    var outputCopy = new iTextSharpPDF.PdfCopy(concatDocument, new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite));
                    concatDocument.Open();
                    try
                    {
                        for (int loop = 0; loop <= inputFiles.GetUpperBound(0); loop++)
                        {
                            var inputDocument = new iTextSharpPDF.PdfReader(inputFiles[loop]);
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


        /// <summary>
        /// Takes pages from two pdf files, and produces an output file
        /// whose odd pages are the contents of the first, and
        /// even pages are the contents of the second. Useful for
        /// merging front/back output from single sided scanners.
        /// </summary>
        /// <param name="oddPageFile">The file supplying odd numbered pages</param>
        /// <param name="evenPageFile">The file supplying even numbered pages</param>
        /// <param name="outputFile">The output file containing the merged pages</param>
        /// <param name="skipExtraPages">Set to true to skip any extra pages if
        ///                              one file is longer than the other</param>
        public void EvenOddMerge(String oddPageFile, String evenPageFile, 
                                 String outputFile, bool skipExtraPages)
        {
            if (!String.IsNullOrEmpty(oddPageFile) && !String.IsNullOrWhiteSpace(oddPageFile) &&
                !String.IsNullOrEmpty(evenPageFile) && !String.IsNullOrWhiteSpace(evenPageFile) &&
                !String.IsNullOrEmpty(outputFile) && !String.IsNullOrWhiteSpace(outputFile))
            {
                var oddPageDocument = new iTextSharpPDF.PdfReader(oddPageFile);
                var evenPageDocument = new iTextSharpPDF.PdfReader(evenPageFile);
                try
                {
                    iTextSharpText.Document mergedOutputDocument = null;
                    iTextSharpPDF.PdfCopy mergedOutputFile = null;

                    int lastPage = 0;
                    switch (skipExtraPages)
                    {
                        case (false):
                            lastPage = oddPageDocument.NumberOfPages;
                            if (evenPageDocument.NumberOfPages > oddPageDocument.NumberOfPages)
                                lastPage = evenPageDocument.NumberOfPages;
                            else
                                lastPage = oddPageDocument.NumberOfPages;
                            break;
                        case (true):
                            if (evenPageDocument.NumberOfPages < oddPageDocument.NumberOfPages)
                                lastPage = evenPageDocument.NumberOfPages;
                            else
                                lastPage = oddPageDocument.NumberOfPages;
                            break;
                    }

                    try
                    {
                        mergedOutputDocument = new iTextSharpText.Document();
                        mergedOutputFile = new iTextSharpPDF.PdfCopy(mergedOutputDocument, new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite));
                        mergedOutputDocument.Open();
                        for (int loop = 1; loop <= lastPage; loop++)
                        {
                            // Extract and merge odd page
                            if (loop <= oddPageDocument.NumberOfPages)
                            {
                                mergedOutputDocument.SetPageSize(oddPageDocument.GetPageSizeWithRotation(loop));
                                mergedOutputFile.AddPage(mergedOutputFile.GetImportedPage(oddPageDocument, loop));
                            }
                            // Extract and merge even page
                            if (loop <= evenPageDocument.NumberOfPages)
                            {
                                mergedOutputDocument.SetPageSize(evenPageDocument.GetPageSizeWithRotation(loop));
                                mergedOutputFile.AddPage(mergedOutputFile.GetImportedPage(evenPageDocument, loop));
                            }
                        }
                    }
                    finally
                    {
                        if (mergedOutputDocument != null && mergedOutputDocument.IsOpen()) mergedOutputDocument.Close();
                        if (mergedOutputFile != null)
                        {
                            mergedOutputFile.Close();
                            mergedOutputFile.FreeReader(oddPageDocument);
                            mergedOutputFile.FreeReader(evenPageDocument);
                        }
                    }
                }
                catch
                {
                    try
                    {
                        File.Delete(outputFile);
                    }
                    catch { }
                    throw;
                }
                finally
                {
                    try
                    {
                        if (oddPageDocument != null) oddPageDocument.Close();
                        oddPageDocument = null;
                    }
                    catch { }
                    try
                    {
                        if (evenPageDocument != null) evenPageDocument.Close();
                        evenPageDocument = null;
                    }
                    catch { }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(oddPageFile) || String.IsNullOrWhiteSpace(oddPageFile)) throw new ArgumentNullException("oddPageFile", exceptionArgumentNullOrEmptyString);
                if (String.IsNullOrEmpty(evenPageFile) || String.IsNullOrWhiteSpace(evenPageFile)) throw new ArgumentNullException("evenPageFile", exceptionArgumentNullOrEmptyString);
                if (String.IsNullOrEmpty(outputFile) || String.IsNullOrWhiteSpace(outputFile)) throw new ArgumentNullException("outputFile", exceptionArgumentNullOrEmptyString);
            }
        }

        /// <summary>
        /// Extracts a range of pages from a PDF file,
        /// and writes them to a new file.
        /// </summary>
        /// <param name="inputFile">The PDF to extract pages from.</param>
        /// <param name="outputFile">The new file to write the extracted pages to.</param>
        /// <param name="firstPage">The first page to extract.</param>
        /// <param name="lastPage">The last page to extract.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks><see cref="FileStream"/> constructor exceptions may also be thrown.</remarks>
        public void ExtractPDFPages(String inputFile, String outputFile, int firstPage, int lastPage)
        {
            if (!String.IsNullOrEmpty(inputFile) && !String.IsNullOrWhiteSpace(inputFile) &&
                !String.IsNullOrEmpty(outputFile) && !String.IsNullOrWhiteSpace(outputFile) &&
                firstPage > 0 && lastPage > 0 &&
                lastPage >= firstPage)
            {
                var inputDocument = new iTextSharpPDF.PdfReader(inputFile);
                try
                {
                    // Page numbers specified must not be greater
                    // than the number of pages in the document 
                    if (firstPage <= inputDocument.NumberOfPages &&
                        lastPage <= inputDocument.NumberOfPages)
                    {
                        iTextSharpText.Document extractOutputDocument = null;
                        iTextSharpPDF.PdfCopy extractOutputFile = null;
                        try
                        {
                            extractOutputDocument = new iTextSharpText.Document();
                            extractOutputFile = new iTextSharpPDF.PdfCopy(extractOutputDocument, new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite));
                            extractOutputDocument.Open();
                            for (int loop = firstPage; loop <= lastPage; loop++)
                            {
                                extractOutputDocument.SetPageSize(inputDocument.GetPageSizeWithRotation(loop));
                                extractOutputFile.AddPage(extractOutputFile.GetImportedPage(inputDocument, loop));
                            }
                        }
                        finally
                        {
                            if (extractOutputDocument != null && extractOutputDocument.IsOpen()) extractOutputDocument.Close();
                            if (extractOutputFile != null)
                            {
                                extractOutputFile.Close();
                                extractOutputFile.FreeReader(inputDocument);
                            }
                        }
                    }
                    else
                    {
                        if (firstPage > inputDocument.NumberOfPages) throw new ArgumentOutOfRangeException("firstPage", String.Format(exceptionParameterCannotBeGreaterThan,"firstPage", "the number of pages in the document."));
                        throw new ArgumentOutOfRangeException("lastPage", String.Format(exceptionParameterCannotBeGreaterThan,"firstPage", "the number of pages in the document."));
                    }

                }
                catch
                {
                    try
                    {
                        File.Delete(outputFile);
                    }
                    catch { }
                    throw;

                }
                finally
                {
                    if (inputDocument != null) inputDocument.Close();
                    inputDocument = null;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(inputFile) || String.IsNullOrWhiteSpace(inputFile)) throw new ArgumentNullException("inputFile", exceptionArgumentNullOrEmptyString);
                if (String.IsNullOrEmpty(outputFile) || String.IsNullOrWhiteSpace(outputFile)) throw new ArgumentNullException("outputFile", exceptionArgumentNullOrEmptyString);
                if (firstPage < 1) throw new ArgumentOutOfRangeException("firstPage", exceptionArgumentZeroOrNegative);
                if (lastPage < 1) throw new ArgumentOutOfRangeException("lastPage", exceptionArgumentZeroOrNegative);
                if (lastPage < firstPage) throw new ArgumentOutOfRangeException("lastPage", String.Format(exceptionParameterCannotBeLessThan, "lastPage", "firstPage"));                
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile">The PDF file to split</param>
        /// <param name="splitStartPages"></param>
        public void SplitPDF(String inputFile, SortedList<int, String> splitStartPages)
        {
            if (!String.IsNullOrEmpty(inputFile) &&
                !String.IsNullOrWhiteSpace(inputFile) &&
                splitStartPages != null &&
                splitStartPages.Count >= 2)
            {
                var inputDocument = new iTextSharpPDF.PdfReader(inputFile);
                // First split must begin with page 1
                // Last split must not be higher than last page
                if (splitStartPages.Keys[0] == 1 &&
                    splitStartPages.Keys[splitStartPages.Count - 1] <= inputDocument.NumberOfPages)
                {
                    int currentPage = 1;
                    int firstPageOfSplit;
                    int lastPageOfSplit;
                    try
                    {
                        for (int splitPoint = 0; splitPoint <= (splitStartPages.Count - 1); splitPoint++)
                        {
                            firstPageOfSplit = currentPage;
                            if (splitPoint < (splitStartPages.Count - 1))
                            {
                                lastPageOfSplit = splitStartPages.Keys[splitPoint + 1] - 1;
                            }
                            else
                            {
                                lastPageOfSplit = inputDocument.NumberOfPages;
                            }
                            iTextSharpText.Document splitDocument = null;
                            iTextSharpPDF.PdfCopy splitOutputFile = null;
                            try
                            {
                                splitDocument = new iTextSharpText.Document();
                                splitOutputFile = new iTextSharpPDF.PdfCopy(splitDocument, new FileStream(splitStartPages.Values[splitPoint], FileMode.Create, FileAccess.ReadWrite));
                                splitDocument.Open();
                                for (int outputPage = firstPageOfSplit; outputPage <= lastPageOfSplit; outputPage++)
                                {
                                    splitDocument.SetPageSize(inputDocument.GetPageSizeWithRotation(currentPage));
                                    splitOutputFile.AddPage(splitOutputFile.GetImportedPage(inputDocument, currentPage));
                                    currentPage++;
                                }
                            }
                            finally
                            {
                                if (splitDocument != null && splitDocument.IsOpen()) splitDocument.Close();
                                if (splitOutputFile != null)
                                {
                                    splitOutputFile.Close();
                                    splitOutputFile.FreeReader(inputDocument);
                                }
                                splitDocument = null;
                                splitOutputFile = null;
                            }
                        }
                    }
                    catch
                    {
                        // Cleanup any files that may have
                        // been written
                        foreach (KeyValuePair<int, String> split in splitStartPages)
                        {
                            try
                            {
                                File.Delete(split.Value);
                            }
                            catch { }
                        }
                        throw;
                    }
                    finally
                    {
                        if (inputDocument != null) inputDocument.Close();
                    }

                }
                else
                {
                    if (splitStartPages.Keys[splitStartPages.Count - 1] > inputDocument.NumberOfPages) throw new ArgumentOutOfRangeException("splitStartPages", String.Format("Final page number must be less than the number of pages ({0}). Passed value is {1}.", inputDocument.NumberOfPages, splitStartPages.Keys[splitStartPages.Count - 1]));
                    throw new ArgumentOutOfRangeException("splitStartPages", "First page number must be 1.");
                }
            }
            else
            {
                if (inputFile == null) throw new ArgumentNullException("inputFile", exceptionArgumentNullOrEmptyString);
                if (splitStartPages == null) throw new ArgumentNullException("splitStartPages", exceptionArgumentNullOrEmptyString);
                throw new ArgumentOutOfRangeException("splitStartPages", "Must contain at least two KeyValue pairs.");
            }
        }

        #endregion

        #region Digital Signature

        
        #endregion
    }
}
