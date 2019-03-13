using System.IO;
using CheckSum.Core.Results;

namespace CheckSum.Core
{
    /// <summary>
    /// Абстрактный класс анализатора файла
    /// Теоретически может быть заменен на интерфейс, но семантически более подходит абстрактный класс
    /// </summary>
    internal abstract class Analyzer
    {
        internal abstract FileResult AnalyzeFile(FileInfo fileInfo);
    }
}