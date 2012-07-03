using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;

namespace PdfExtract
{
    public class Program
    {
        private const string messageFileNotFound = "{0} not found or inaccessible.";

        private const string messageNoInputFileSpecifed = "No input file(s) specified.";
        private const string messageNoExtractPagesSpecifed = "No extract pages specified.";
        private const string messageInvalidExtractPage = "Invalid extract page: {0}";
        private const string messageInvalidExtractRange = "Invalid extract range: {0}";

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
                        if (commandLineOptions.DebugMessages)
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
            if (commandLineOptions.Items.Count > 0)
            {
                // Make sure user has specified pages to extract
                if (commandLineOptions.ExtractPages != null && commandLineOptions.ExtractPages.Count > 0)
                {
                    // Make sure the input file can actually be
                    // opened

                    validatedOK = true;
                }
                else
                {
                    // No extract pages specified
                    errorMessage.Append(messageNoExtractPagesSpecifed);
                }
            }
            else
            {
                errorMessage.Append(messageNoInputFileSpecifed);
            }
            if (!validatedOK) System.Console.Error.WriteLine(errorMessage.ToString());
            return validatedOK;
        }

    }
}
