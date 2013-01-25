using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CommandLine;
using PdfMiniToolsCore;

using iTextSharpText = iTextSharp.text;
using iTextSharpPDF = iTextSharp.text.pdf;

namespace PdfEvenOddMerge
{
    public class TaskProcessor
    {
        #region Ctor
        public TaskProcessor()
        { }
        #endregion

        public void ProcessTask(Options commandLineOptions)
        {
            var splitTools = new CoreTools();
            try
            {
                splitTools.EvenOddMerge(commandLineOptions.Items[0],
                                        commandLineOptions.Items[1],
                                        commandLineOptions.Items[2],
                                        commandLineOptions.skipExtraPages);
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
                    // Some other IOException we weren't expecting
                    throw;
                }
            }
        }

    }
}
