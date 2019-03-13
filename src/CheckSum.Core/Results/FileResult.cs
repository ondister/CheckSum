using System;
using System.Xml.Serialization;

namespace CheckSum.Core.Results
{
    /// <summary>
    /// Класс результата подсчета суммы байт файла
    /// </summary>
    [Serializable]
    public class FileResult
    {
        [XmlAttribute("File_name")] public string FileName { get; set; }

        [XmlAttribute("Bytes_checksum")] public long CheckSum { get; set; }
    }
}