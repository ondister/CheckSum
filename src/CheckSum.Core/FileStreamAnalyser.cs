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

                fileResult = new FileResultSuccess
                {
                    CheckSum = checkSum,
                    FileName = fileInfo.FullName,
                    Status = FileAnalyzeStatus.Success
                };
            }
            catch (IOException)
            {
                fileResult = new FileResultError
                {
                    ErrorMessage = "Ошибка ввода вывода",
                    FileName = fileInfo.FullName,
                    Status=FileAnalyzeStatus.Error
                };
            }
            catch (UnauthorizedAccessException)
            {
                fileResult = new FileResultError
                {
                    ErrorMessage = "Ошибка доступа к файлу",
                    FileName = fileInfo.FullName,
                    Status = FileAnalyzeStatus.Error
                };
            }
            catch
            {
                fileResult = new FileResultError
                {
                    ErrorMessage = "Ошибка чтения потока",
                    FileName = fileInfo.FullName,
                    Status = FileAnalyzeStatus.Error
                };
            }

            return fileResult;
        }
    }
}