using System;
using System.IO;
using CheckSum.Core.Results;

namespace CheckSum.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     Реализация анализа файла через побитовое чтение с использованием FileStream
    ///     Можно реализовать асинхронное чтение большого файла по частям, но мне кажется это не даст нужной
    ///     производительности, а наоборот ее ухудшит
    /// </summary>
    internal sealed class FileStreamAnalyser : Analyzer
    {
        /// <summary>
        ///     Считает сумму значений байт для указанного файла
        /// </summary>
        /// <param name="fileInfo">FileInfo файла</param>
        /// <returns></returns>
        internal override FileResult AnalyzeFile(FileInfo fileInfo)
        {
            const int zeroFileLenghtCheckSum = -1;
            var checkSum = 0L;
            FileResult fileResult;
            try
            {
                using (var fileStream = fileInfo.OpenRead())
                {
                    var byteValue = 0;
                    do
                    {
                        byteValue = fileStream.ReadByte();

                        checkSum += byteValue;
                    } while (byteValue != -1);
                }

                //если длина файла равна 0
                if (checkSum == zeroFileLenghtCheckSum)
                {
                    fileResult = new FileResultZeroSum
                    {
                        FileName = fileInfo.FullName
                    };
                }
                else
                {
                    fileResult = new FileResultSuccess
                    {
                        CheckSum = checkSum,
                        FileName = fileInfo.FullName
                    };
                }
            }
            catch (IOException)
            {
                fileResult = new FileResultError
                {
                    ErrorMessage = "Ошибка ввода вывода",
                    FileName = fileInfo.FullName
                };
            }
            catch (UnauthorizedAccessException)
            {
                fileResult = new FileResultError
                {
                    ErrorMessage = "Ошибка доступа к файлу",
                    FileName = fileInfo.FullName
                };
            }
            catch
            {
                fileResult = new FileResultError
                {
                    ErrorMessage = "Ошибка чтения потока",
                    FileName = fileInfo.FullName
                };
            }

            return fileResult;
        }
    }
}