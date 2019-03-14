using System;
using System.Xml.Serialization;

namespace CheckSum.Core.Results
{
    /// <inheritdoc />
    /// <summary>
    /// Класс результата обработки файла с ошибками
    /// </summary>
    [Serializable]
    public class FileResultError : FileResult
    {
        [XmlAttribute("Bytes_checksum")] public string ErrorMessage { get; set; }
        public override string GetStringResult()
        {
            return $"Error of the file {FileName} analis: {ErrorMessage}";
        }
    }
}