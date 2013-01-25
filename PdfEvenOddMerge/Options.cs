using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace PdfEvenOddMerge
{
    public class Options
    {

        [Option("s", "skip", Required = false, HelpText = "Skip extra pages.")]
        public bool skipExtraPages = false; 

        [Option("d", "debug", Required = false, HelpText = "Display details of any unhandled exceptions. Default is false.")]
        public bool debugMessages = false;


        [HelpOption(HelpText = "Display this help text.")]
        public String ShowUsage()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("Usage:");
            helpMessage.AppendLine("\n   pdfevenoddmerge [-s] oddfile evenfile outputfile");
            helpMessage.AppendLine("\nExample 1:");
            helpMessage.AppendLine("\n   pdfevenoddmerge file1.pdf file2.pdf combined.pdf");
            helpMessage.AppendLine("\nCreates the file combined.pdf, with odd pages from file1.pdf,\nand even pages from file2.pdf.");
            helpMessage.AppendLine("\nExample 2:");
            helpMessage.AppendLine("\n   pdfevenoddmerge -s file1.pdf file2.pdf combined.pdf");
            helpMessage.AppendLine("\nSame as Example 1, except any extra pages will be skipped.");
            helpMessage.AppendLine("\n(ie if file1.pdf contains 10 pages, and file2.pdf contains 12, ");
            helpMessage.AppendLine("\npages 11 and 12 in file2.pdf will be ignored.");

            return helpMessage.ToString();
        }

        [ValueList(typeof(List<string>))]
        public IList<string> Items = null;

    }
}
