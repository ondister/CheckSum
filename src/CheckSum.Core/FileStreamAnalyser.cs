using System.IO;
using CheckSum.Core.Results;

namespace CheckSum.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Реализация анализа файла через побитовое чтение с использованием FileStream
    /// Можно реализовать асинхронное чтение большого файла по частям, но мне кажется это не даст нужной производительности, а наоборот ее ухудшит
    /// </summary>
    internal sealed class FileStreamAnalyser : Analyzer
    {
        internal override FileResult AnalyzeFile(FileInfo fileInfo)
        {
            var checkSum = 0L;

            using (var fileStream = fileInfo.OpenRead())
            {
                var byteValue = 0;
                do
                {
                    byteValue = fileStream.ReadByte();

                    checkSum += byteValue;
                } while (byteValue != -1);
            }


            var fileResult = new FileResult
            {
                CheckSum = checkSum,
                FileName = fileInfo.FullName
            };

            return fileResult;
        }
    }
}