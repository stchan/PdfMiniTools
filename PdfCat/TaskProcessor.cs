using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using PdfMiniToolsCore;

namespace PdfCat
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
            List<String> inputFiles = new List<string>(commandLineOptions.Items);
            String outputFile = inputFiles[inputFiles.Count - 1];
            inputFiles.RemoveAt(inputFiles.Count - 1);
            try
            {
                pdfTools.ConcatenatePDFFiles(inputFiles.ToArray(), outputFile);
            }
            catch (UnauthorizedAccessException)
            {
                System.Console.Error.WriteLine("Access denied.");
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Console.Error.WriteLine("File not found.");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                System.Console.Error.WriteLine("Directory not found.");
            }
            
        }

    }
}
