using System;
using System.Collections.Generic;
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
            PdfMiniToolsCore.CoreTools splitTools = new CoreTools();
        }
    }
}
