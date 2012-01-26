using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace PdfCat
{
    public class Options
    {

        [Option("d", "debug", Required = false, HelpText = "Display details of any unhandled exceptions. Default is false.")]
        public bool DebugMessages = false;

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
