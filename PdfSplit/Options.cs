using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace PddSplit
{
    public class Options
    {

        [Option("d", "debug", Required = false, HelpText = "Display details of any unhandled exceptions. Default is false.")]
        public bool DebugMessages = false;


        //[Option("s", "splits", Required = true, HelpText = "Pages to split at.")]
        [OptionList("s", "splits", ',', Required=true, HelpText="Pages to split at, separated by a comma.")]
        public String SplitPages = String.Empty;


        [HelpOption(HelpText = "Display this help text.")]
        public String ShowUsage()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("Usage:");
            helpMessage.AppendLine("\n   pdfcat file file2 [file3...] outputfile");
            helpMessage.AppendLine("\nExample:");
            helpMessage.AppendLine("\n   pdfcat file1.pdf file2.pdf concat.pdf");
            helpMessage.AppendLine("\nConcatenates file1.pdf and file2.pdf into concat.pdf");

            return helpMessage.ToString();
        }

        [ValueList(typeof(List<string>))]
        public IList<string> Items = null;

    }
}
