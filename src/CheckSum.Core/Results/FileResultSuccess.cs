using System;
using System.Xml.Serialization;

namespace CheckSum.Core.Results
{
    /// <inheritdoc />
    /// <summary>
    /// Класс успешного результата обработки файла
    /// </summary>
    [Serializable]
    public class FileResultSuccess:FileResult
    {
        [XmlAttribute("Bytes_checksum")] public long CheckSum { get; set; }

        public override string GetStringResult()
        {
            return $"File:{FileName}, bytes sum: {CheckSum}";
        }

        public FileResultSuccess()
        {
            Status = FileAnalyzeStatus.Success;
        }
    }
}