using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace CheckSum.Core.Results
{
    /// <summary>
    /// Класс результата обработки дерева папок
    /// </summary>
    [Serializable]
    public class FolderResult
    {
        public FolderResult()
        {
            FilesCollection = new Collection<FileResult>();
        }

        [XmlElement("Files")] public Collection<FileResult> FilesCollection { get; set; }

        internal void Save(string path)
        {
            var formatter = new XmlSerializer(typeof(FolderResult));

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fileStream, this);
            }
        }
    }
}