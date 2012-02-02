using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace PdfSplit
{
    public class Options
    {

        [Option("d", "debug", Required = false, HelpText = "Display details of any unhandled exceptions. Default is false.")]
        public bool DebugMessages = false;

        [Option("p", "prefix", Required = false, HelpText = "Sets the prefix of outputfiles. Default is the input filename.")]
        public String OutputFilePrefix = null;

        [OptionList("s", "splits", Required = true, Separator = ',', HelpText = "Pages to split at, separated by a comma.")]
        public IList<String> SplitPages = null;


        [HelpOption(HelpText = "Display this help text.")]
        public String ShowUsage()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("Usage:");
            helpMessage.AppendLine("\n   pdfsplit -s page1[,page2] [-p prefix] file");
            helpMessage.AppendLine("\nExample:");
            helpMessage.AppendLine("\n   pdfsplit -s 11,21 -p splitfile file1.pdf");
            helpMessage.AppendLine("\nSplits file1.pdf at pages 11, and 21 into three files:");
            helpMessage.AppendLine("\n   splitfile1.pdf (contains pages 1-10)");
            helpMessage.AppendLine("\n   splitfile2.pdf (contains pages 11-20)");
            helpMessage.AppendLine("\n   splitfile3.pdf (contains all remaining pages)");
            return helpMessage.ToString();
        }

        [ValueList(typeof(List<string>))]
        public IList<string> Items = null;

    }
}
