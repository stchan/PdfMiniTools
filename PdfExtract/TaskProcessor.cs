using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CommandLine;
using PdfMiniToolsCore;

namespace PdfExtract
{
    public class TaskProcessor
    {
        #region Ctor

        public TaskProcessor()
        { }

        #endregion

        public void ProcessTask(Options commandLineOptions)
        {
            var pdfTools = new CoreTools();
            try
            {
                Regex singlePage = new Regex(@"^\d+$", RegexOptions.IgnorePatternWhitespace);
                Regex pageRange = new Regex(@"^\d+-\d+$", RegexOptions.IgnorePatternWhitespace);
                String outputPrefix;
                if (!String.IsNullOrEmpty(commandLineOptions.OutputFilePrefix))
                {
                    outputPrefix = commandLineOptions.OutputFilePrefix;
                }
                else
                {
                    outputPrefix = Path.GetFileNameWithoutExtension(commandLineOptions.Items[0]);
                }
                int[] extractPages = { 0, 0 };
                for (int loop = 0; loop < commandLineOptions.ExtractPages.Count; loop++)
                {
                    if (pageRange.IsMatch(commandLineOptions.ExtractPages[loop]))
                    {
                        String[] extractRange = commandLineOptions.ExtractPages[loop].Split('-');
                        extractPages[0] = Convert.ToInt32(extractRange[0]);
                        extractPages[1] = Convert.ToInt32(extractRange[1]);
                    }
                    else
                    {
                        extractPages[0] = Convert.ToInt32(commandLineOptions.ExtractPages[loop]);
                        extractPages[1] = Convert.ToInt32(commandLineOptions.ExtractPages[loop]);
                    }
                    pdfTools.ExtractPDFPages(commandLineOptions.Items[0],
                                             outputPrefix + "_" + (loop + 1).ToString() + ".PDF",
                                             extractPages[0],
                                             extractPages[1]);
                }
            }
            catch (System.IO.IOException ioException)
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
            catch (ArgumentOutOfRangeException argException)
            {
                if (argException.Message.Contains("the number of pages in the document"))
                {
                    System.Console.Error.WriteLine("A page after the last page was specified.");
                }
                else if (argException.Message.Contains("Parameter cannot be zero or negative"))
                {
                    System.Console.Error.WriteLine("A page number less than one was specified.");
                }


            }
            catch (UnauthorizedAccessException)
            {
                System.Console.Error.WriteLine("Access denied.");
            }
        }
    }
}
