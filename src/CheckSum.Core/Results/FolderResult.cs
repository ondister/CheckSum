using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace CheckSum.Core.Results
{
    /// <summary>
    ///     Класс результата обработки дерева папок
    /// </summary>
    [Serializable]
    public class FolderResult
    {
        public FolderResult()
        {
            FilesCollection = new Collection<FileResult>();
        }

        [XmlElement("Files")] public Collection<FileResult> FilesCollection { get; set; }

        /// <summary>
        ///     Cохраняет результат по указанному пути
        /// </summary>
        /// <param name="fileName">Полный путь для сохранения файла</param>
        public void Save(string fileName)
        {
            var formatter = new XmlSerializer(typeof(FolderResult));

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fileStream, this);
            }
        }

        /// <summary>
        ///     Открывает результат из указанного пути.
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <returns></returns>
        public static FolderResult Open(string fileName)
        {
            FolderResult folderResult = null;
            var formatter = new XmlSerializer(typeof(FolderResult));
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                var resultObject = formatter.Deserialize(fileStream);
                folderResult = resultObject as FolderResult;
            }

            return folderResult;
        }
    }
}