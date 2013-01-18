using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;

namespace PdfEvenOddMerge
{
    public class Program
    {

        private const string messageFileNotFound = "{0} not found or inaccessible.";
        private const string messageCouldNotCreateFile = "Could not create {0}";

        private const string messageNoFilesSpecified = "No input or output files were specified.";
        private const string messageNoInputFileSpecifed = "No input files specified.";
        private const string messageNoOutputFileSpecifed = "No output file specified.";
        private const string messageInsufficientInputFiles = "At least two files to concatenate must be specified.";

        private const string messageUnexpectedError = "There was an unexpected internal error.";
        private const string messageUnhandledException = "Exception: {0}\r\nMessage:{1}\r\nStack Trace:{2}";

        public static void Main(string[] args)
        {
            Options commandLineOptions = new Options();
            ICommandLineParser commandParser = new CommandLineParser();
            if (commandParser.ParseArguments(args, commandLineOptions, Console.Error))
            {
                if (ValidateOptions(commandLineOptions))
                {
                    try
                    {
                        TaskProcessor concatTask = new TaskProcessor();
                        concatTask.ProcessTask(commandLineOptions);
                    }
                    catch (Exception ex)
                    {
                        StringBuilder errorMessage = new StringBuilder();
                        errorMessage.AppendLine(messageUnexpectedError);
                        if (commandLineOptions.debugMessages)
                        {
                            errorMessage.AppendFormat(messageUnhandledException, ex.ToString(), ex.Message, ex.StackTrace);
                        }
                        System.Console.Error.WriteLine(errorMessage.ToString());
                        Environment.ExitCode = 1;
                    }
                }
            }
            else
            {
                // Command line params could not be parsed,
                // or help was requested
                Environment.ExitCode = -1;
            }
        }

        private static bool ValidateOptions(Options commandLineOptions)
        {
            bool validatedOK = false;
            StringBuilder errorMessage = new StringBuilder();

            return validatedOK;
        }
    }
}
