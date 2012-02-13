using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CommandLine;
using PdfMiniToolsCore;

namespace PdfSplit
{
    public class TaskProcessor
    {
        public void ProcessTask(Options commandLineOptions)
        {
            String inputFile = commandLineOptions.Items[0];
            var splitTools = new CoreTools();
            List<String> splitPages;
            switch (commandLineOptions.allPages)
            {
                case (true):
                    // Splitting out every single page,
                    // so there has to be any entry for
                    // every page in the List object
                    Dictionary<String, String> pdfInfo = splitTools.RetrieveBasicProperties(inputFile);
                    int pageCount = Convert.ToInt32(pdfInfo["Page Count"]);
                    splitPages = new List<string>();
                    for (int loop = 2; loop <= pageCount; loop++)
                    {
                        splitPages.Add(loop.ToString());
                    }
                    break;
                default:
                    splitPages = commandLineOptions.SplitPages.Distinct().ToList<String>();
                    splitPages.Sort();
                    break;
            }
            String outputFilePrefix;
            if (!String.IsNullOrWhiteSpace(commandLineOptions.OutputFilePrefix))
            {
                outputFilePrefix = commandLineOptions.OutputFilePrefix;
            }
            else
            {
                if (!String.IsNullOrEmpty(Path.GetDirectoryName(commandLineOptions.Items[0])))
                {
                    outputFilePrefix = Path.Combine(Path.GetDirectoryName(commandLineOptions.Items[0]),Path.GetFileNameWithoutExtension(commandLineOptions.Items[0]));
                }
                else
                {
                    outputFilePrefix = Path.GetFileNameWithoutExtension(commandLineOptions.Items[0]);
                }
            }
            outputFilePrefix += "{0:" + new String('0', splitPages.Count.ToString().Length) + "}.PDF";
            var splitStartPages = new SortedList<int, String>();
            
            for (int loop = 0; loop < splitPages.Count; loop++)
            {
                splitStartPages.Add(Convert.ToInt32(splitPages[loop]), String.Format(outputFilePrefix, loop + 2));
            }
            if (!splitStartPages.ContainsKey(1)) splitStartPages.Add(1, String.Format(outputFilePrefix, 1)); // Add page 1 if not specified by user (it usually isn't)
            try
            {
                splitTools.SplitPDF(inputFile, splitStartPages);
            }
            catch (ArgumentOutOfRangeException outOfRangeException)
            {
                // Page 0 or page number greater than # of pages in PDF specified
                String consoleMessage = outOfRangeException.Message.Remove(outOfRangeException.Message.LastIndexOf("Parameter name:", StringComparison.CurrentCultureIgnoreCase));
                System.Console.Error.WriteLine(Environment.NewLine + consoleMessage);
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
            catch (System.IO.DirectoryNotFoundException)
            {
                System.Console.Error.WriteLine(Environment.NewLine + "Directory not found.");
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
    }
}
