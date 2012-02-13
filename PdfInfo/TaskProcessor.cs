using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;

using CommandLine;
using PdfMiniToolsCore;

namespace PdfInfo
{
    public class TaskProcessor
    {
        private const String reportNoPdfInfo = "No valid info could be extracted from the PDF file.";

        private const String reportMessageCSVOutput = "\"{0}\", \"{1}\"";
        private const String reportMessageStandardOutput = "{0}: {1}";

        public void ProcessTask(Options commandLineOptions)
        {
            String inputFile = commandLineOptions.Items[0];
            var infoTools = new CoreTools();
            Dictionary<String, String> pdfInfo = new Dictionary<String, String>();
            try
            {
                if (commandLineOptions.showAll || commandLineOptions.showBasic)
                {
                    foreach (KeyValuePair<String, String> pdfInfoPair in infoTools.RetrieveBasicProperties(inputFile))
                    {
                        pdfInfo.Add(pdfInfoPair.Key, pdfInfoPair.Value);
                    }
                    foreach (KeyValuePair<String, String> pdfInfoPair in infoTools.RetrieveInfo(inputFile))
                    {
                        pdfInfo.Add(pdfInfoPair.Key, pdfInfoPair.Value);
                    }
                }
                if (commandLineOptions.showAll || commandLineOptions.showFields)
                {
                    foreach (KeyValuePair<String, String> pdfInfoPair in infoTools.RetrieveAcroFieldsData(inputFile))
                    {
                        pdfInfo.Add(pdfInfoPair.Key, pdfInfoPair.Value);
                    }
                }
                WriteResults(commandLineOptions.csvOutput, pdfInfo);
            }
            catch (ArgumentException argException)
            {
                // Output file prefix (-p) contains illegal characters.
                if (argException.Message.Contains("Illegal characters in path"))
                    System.Console.Error.WriteLine(Environment.NewLine + argException.Message);
            }
            catch (UnauthorizedAccessException)
            {
                System.Console.Error.WriteLine(Environment.NewLine + "Access denied.");
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Console.Error.WriteLine(Environment.NewLine + "File not found.");
            }
            catch (IOException ioException)
            {
                // PDF file is not valid, or was not found
                if (ioException.Message.Contains("PDF"))
                {
                    System.Console.Error.WriteLine(Environment.NewLine + "Input file is not a valid PDF.");
                }
                else if (ioException.Message.Contains("not found as file or resource"))
                {
                    System.Console.Error.WriteLine(Environment.NewLine + ioException.Message);
                }
                else
                {
                    throw;
                }
            }

        }

        private void WriteResults(bool csvOutput, Dictionary<String, String> pdfInfo)
        {
            if (pdfInfo.Count > 0)
            {
                IDictionaryEnumerator infoEnumerator = pdfInfo.GetEnumerator();
                while (infoEnumerator.MoveNext())
                {
                    switch (csvOutput)
                    {
                        case (true):
                            System.Console.WriteLine(String.Format(reportMessageCSVOutput, infoEnumerator.Key, infoEnumerator.Value));
                            break;
                        default:
                            System.Console.WriteLine(String.Format(reportMessageStandardOutput, infoEnumerator.Key, infoEnumerator.Value));
                            break;
                    }
                }
            }
            else
            {
                System.Console.WriteLine(reportNoPdfInfo);
            }
        }
        
    }
}
