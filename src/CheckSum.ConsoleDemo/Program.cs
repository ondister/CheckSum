using System;
using System.Collections.Generic;
using CheckSum.Core;
using CheckSum.Core.Results;

namespace CheckSum.ConsoleDemo
{
    internal class Program
    {
        private const string testFolderPath = @"..\..\TestFiles";

        private static void Main(string[] args)
        {
            var folderPath = GetfolderPath(args);
            try
            {
                var checkSumFolder = new FolderCheckSum(folderPath);

                //end of analysis event
                checkSumFolder.Analized += CheckSumFolder_Analized;

                //progress of analysis event
                var progress = new Progress<FileResult>();
                progress.ProgressChanged += Progress_ProgressChanged;

                checkSumFolder.AnalizeAsync(progress).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void CheckSumFolder_Analized(object sender, string e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Folder {e} analysis completed");
            Console.ReadKey();
        }


        private static void Progress_ProgressChanged(object sender, FileResult e)
        {
            Console.WriteLine(e.GetStringResult());
        }

        private static string GetfolderPath(IReadOnlyList<string> args)
        {
            var folderPath = testFolderPath;
            if (args.Count != 1)
            {
                Console.WriteLine(
                    $"You did not specify the path to the folder in the program arguments. The default path will be used: {testFolderPath}");
            }
            else
            {
                folderPath = args[0];
            }

            return folderPath;
        }
    }
}