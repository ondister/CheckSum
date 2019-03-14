using System;
using System.Xml.Serialization;

namespace CheckSum.Core.Results
{
    /// <summary>
    /// Базовый класс результата обработки файла
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(FileResultSuccess))]
    [XmlInclude(typeof(FileResultError))]
    public abstract class FileResult
    {
        /// <summary>
        /// Статус обработки файла
        /// </summary>
        [XmlAttribute("Status")] public string Status { get; set; }

        /// <summary>
        /// Полный путь к файлу
        /// </summary>
        [XmlAttribute("File_name")] public string FileName { get; set; }
        
        /// <summary>
        /// Возвращает результат в виде строки
        /// </summary>
        /// <returns></returns>
        public abstract string GetStringResult();
    }
}