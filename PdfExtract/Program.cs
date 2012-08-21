using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        private const string messageInvalidExtractPageOrRange = "Invalid extract page or range: {0}";

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
                        if (commandLineOptions.DebugMessages == true)
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
                    try
                    {
                        using (FileStream inputFile = new FileStream(commandLineOptions.Items[0], FileMode.Open, FileAccess.Read))
                        {
                            inputFile.Close();
                        }
                    }
                    catch
                    {
                        errorMessage.AppendLine(String.Format(messageFileNotFound, commandLineOptions.Items[0]));
                    }
                    if (errorMessage.Length == 0)
                    {
                        // Validate the extract page parameters
                        Regex singlePage = new Regex(@"^\d+$", RegexOptions.IgnorePatternWhitespace);
                        Regex pageRange = new Regex(@"^\d+-\d+$", RegexOptions.IgnorePatternWhitespace);
                        foreach (String extractPageParameter in commandLineOptions.ExtractPages)
                        {
                            if (!singlePage.IsMatch(extractPageParameter))
                            {
                                if (!pageRange.IsMatch(extractPageParameter))
                                {
                                    // Parameter is neither a valid page
                                    // nor a valid page range
                                    errorMessage.AppendLine(String.Format(messageInvalidExtractPageOrRange, extractPageParameter));
                                    break;
                                }
                                else
                                {
                                    // Valid range format
                                    // Make sure the start page in the range
                                    // is less than the end page, and that
                                    // neither page is zero
                                    String[] extractPages = extractPageParameter.Split('-');
                                    int startPage, endPage;
                                    if (!(Int32.TryParse(extractPages[0], out startPage) && Int32.TryParse(extractPages[1], out endPage)
                                        && (endPage >= startPage) && startPage >= 1 && endPage >= 1))
                                    {
                                        errorMessage.AppendLine(String.Format(messageInvalidExtractRange, extractPageParameter));
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Make sure single page is not zero
                                int extractPage;
                                if (!(Int32.TryParse(extractPageParameter, out extractPage) &&
                                    extractPage >= 1))
                                {
                                    errorMessage.AppendLine(String.Format(messageInvalidExtractPageOrRange, extractPageParameter));
                                    break;
                                }
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(errorMessage.ToString())) validatedOK = true;
                }
                else
                {
                    // No extract pages specified
                    errorMessage.Append(messageNoExtractPagesSpecifed);
                }
            }
            else
            {
                // No input file specified
                errorMessage.Append(messageNoInputFileSpecifed);
            }
            if (!validatedOK) System.Console.Error.WriteLine(errorMessage.ToString());
            return validatedOK;
        }

    }
}
