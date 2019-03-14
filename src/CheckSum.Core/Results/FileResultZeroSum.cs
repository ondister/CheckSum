using System;

namespace CheckSum.Core.Results
{
    /// <inheritdoc />
    /// <summary>
    ///     Класс  результата обработки файла с нулевым размером
    /// </summary>
    [Serializable]
    public class FileResultZeroSum : FileResult
    {
        public FileResultZeroSum()
        {
            Status = FileAnalyzeStatus.ZeroFile;
        }

        public override string GetStringResult()
        {
            return $"File:{FileName} have zero size";
        }
    }
}