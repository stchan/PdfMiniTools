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

        [OptionList("a", "allpages", Required = false, Separator = ',', 
                    HelpText = "Pages to split at, separated by a comma.",
                    MutuallyExclusiveSet="SplitPages")]
        public Boolean allPages = false;

        [OptionList("s", "splits", Required = false, Separator = ',', 
                    HelpText = "Pages to split at, separated by a comma.",
                    MutuallyExclusiveSet = "SplitPages")]
        public IList<String> SplitPages = null;


        [HelpOption(HelpText = "Display this help text.")]
        public String ShowUsage()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("Usage:");
            helpMessage.AppendLine("\n   pdfsplit -{a|s} [page1,page2] [-p prefix] file");
            helpMessage.AppendLine("\nExample:");
            helpMessage.AppendLine("\n   pdfsplit -s 11,21 -p splitfile file1.pdf");
            helpMessage.AppendLine("\nSplits file1.pdf at pages 11, and 21 into three files:\n");
            helpMessage.AppendLine("   splitfile01.pdf (contains pages 1-10)");
            helpMessage.AppendLine("   splitfile02.pdf (contains pages 11-20)");
            helpMessage.AppendLine("   splitfile03.pdf (contains all remaining pages)");
            helpMessage.AppendLine("\nExample 2:");
            helpMessage.AppendLine("\n   pdfsplit -a file.pdf");
            helpMessage.AppendLine("\nSplits file.pdf at every page.\n");
            helpMessage.AppendLine("   file01.pdf (page 1)");
            helpMessage.AppendLine("   file02.pdf (page 2)");
            helpMessage.AppendLine("   file03.pdf (page 3)");
            helpMessage.AppendLine("       .");
            helpMessage.AppendLine("       .");
            helpMessage.AppendLine("   fileXX.pdf (page XX - last page)");
            return helpMessage.ToString();
        }

        [ValueList(typeof(List<string>))]
        public IList<string> Items = null;

    }
}
