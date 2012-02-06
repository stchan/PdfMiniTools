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
            List<String> splitPages = commandLineOptions.SplitPages.Distinct().ToList<String>();
            splitPages.Sort();
            String outputFilePrefix;
            if (!String.IsNullOrWhiteSpace(commandLineOptions.OutputFilePrefix))
            {
                outputFilePrefix = commandLineOptions.OutputFilePrefix;
            }
            else
            {
                if (!String.IsNullOrEmpty(Path.GetDirectoryName(commandLineOptions.Items[0])))
                {
                    outputFilePrefix = Path.GetDirectoryName(commandLineOptions.Items[0]) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(commandLineOptions.Items[0]);
                }
                else
                {
                    outputFilePrefix = Path.GetFileNameWithoutExtension(commandLineOptions.Items[0]);
                }
            }
            var splitStartPages = new SortedList<int, String>();
            splitStartPages.Add(1, outputFilePrefix + "1.PDF");
            for (int loop = 0; loop < splitPages.Count; loop++)
            {
                splitStartPages.Add(Convert.ToInt32(splitPages[loop]), outputFilePrefix + (loop + 2).ToString() + ".PDF");
            }
            var splitTools = new CoreTools();
            splitTools.SplitPDF(inputFile, splitStartPages); 
        }
    }
}
