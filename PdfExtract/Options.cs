using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace PdfExtract
{
    public class Options
    {

        [Option("d", "debug", Required = false, HelpText = "Display details of any unhandled exceptions. Default is false.")]
        public bool DebugMessages = false;

        [OptionList("e", "extract", Required = true, Separator = ',',
            HelpText = "Pages to extract, separated by a comma.",
            MutuallyExclusiveSet = "ExtractPages")]
        public IList<String> ExtractPages = null;

        [Option("p", "prefix", Required = false, HelpText = "Sets the prefix of outputfiles. Default is the input filename.")]
        public String OutputFilePrefix = null;

        [HelpOption(HelpText = "Display this help text.")]
        public String ShowUsage()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("Usage:");
            helpMessage.AppendLine("\n   pdfextract -e page1[,page2] [-p prefix] inputfile ");
            helpMessage.AppendLine("\nExample:");
            helpMessage.AppendLine("\n   pdfextract -e 12,16,23 inputfile.pdf");
            helpMessage.AppendLine("\nExtracts pages 12,16, and 23 from inputfile.pdf");

            return helpMessage.ToString();
        }

        [ValueList(typeof(List<string>))]
        public IList<string> Items = null;

    }
}
