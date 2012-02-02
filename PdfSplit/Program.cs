using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;

namespace PdfSplit
{
    public class Program
    {
        static void Main(string[] args)
        {
            Options commandLineOptions = new Options();
            ICommandLineParser commandParser = new CommandLineParser();
            if (commandParser.ParseArguments(args, commandLineOptions, Console.Error))
            {
            }
            else
            {
                // Command line params could not be parsed,
                // or help was requested
                Environment.ExitCode = -1;
            }

        }

        private bool ValidateOptions(Options commandLineOptions)
        {
            bool validatedOK = false;


            return validatedOK;
        }
    }
}
