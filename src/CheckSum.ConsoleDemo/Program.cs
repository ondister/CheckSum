using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckSum.Core;
using CheckSum.Core.Results;

namespace CheckSum.ConsoleDemo
{
    class Program
    {
        private const string existFolderPath = @"..\..\TestFiles";
#warning не забыть сделать передачу аргументов из коммандной строки
        static void Main(string[] args)
        {
          var checkSumFolder= new FolderCheckSum(existFolderPath);
            checkSumFolder.Analized += delegate
            {
                Console.WriteLine("Анализ папки завершен");
            };
            var progress = new Progress<FileResult>();
            progress.ProgressChanged += Progress_ProgressChanged1;
            checkSumFolder.AnalizeAsync(progress).Wait();

           Console.ReadKey();
        }

        private static void Progress_ProgressChanged1(object sender, FileResult e)
        {
            Console.WriteLine($"Файл:{e.FileName}, сумма байт: {e.CheckSum}");
        }

    }
}
