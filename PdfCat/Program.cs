using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CommandLine;
using PdfMiniToolsCore;

namespace PdfCat
{
    public class Program
    {
        

        const string messageFileNotFound = "{0} not found or inaccessible.";
        const string messageCouldNotCreateFile = "Could not create {0}";

        const string messageNoFilesSpecified = "No input or output files were specified.";
        const string messageNoInputFileSpecifed = "No input file(s) specified.";
        const string messageInsufficientInputFiles = "At least two files to concatenate must be specified.";

        const string messageUnexpectedError = "There was an unexpected internal error.";
        const string messageUnhandledException = "Exception: {0}\r\nMessage:{1}\r\nStack Trace:{2}";

        static void Main(string[] args)
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

            if (commandLineOptions.Items.Count > 2)
            {
                // Make sure the input files are actually readable
                for (int loop = 0; loop < commandLineOptions.Items.Count - 1; loop++)
                {
                    try
                    {
                        using (FileStream inputFile = new FileStream(commandLineOptions.Items[loop], FileMode.Open, FileAccess.Read))
                        {
                            inputFile.Close();
                        }
                    }
                    catch
                    {
                        errorMessage.AppendLine(String.Format(messageFileNotFound, commandLineOptions.Items[loop]));
                    }

                }
                // Make sure we can actually write the desired output file
                try
                {
                    using (FileStream outputFile = new FileStream(commandLineOptions.Items[commandLineOptions.Items.Count - 1], FileMode.Create, FileAccess.ReadWrite))
                    {
                        outputFile.Close();
                    }

                }
                catch
                {
                    errorMessage.AppendLine(String.Format(messageCouldNotCreateFile, commandLineOptions.Items[commandLineOptions.Items.Count - 1]));
                }
                finally
                {
                    try
                    {
                        File.Delete(commandLineOptions.Items[commandLineOptions.Items.Count - 1]);
                    }
                    catch { }
                }
                if (String.IsNullOrEmpty(errorMessage.ToString())) validatedOK = true; 
            }
            else
            {
                // Only one (or no) file(s) specified
                switch (commandLineOptions.Items.Count)
                {
                    case 2:
                        errorMessage.AppendLine(messageInsufficientInputFiles);
                        break;
                    case 1:
                        errorMessage.AppendLine(messageNoInputFileSpecifed);
                        break;
                    default:
                        errorMessage.AppendLine(messageNoFilesSpecified);
                        break;
                }
            }

            if (!validatedOK) System.Console.Error.Write(errorMessage.ToString());

            return validatedOK;
        }


    }
}
