using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CheckSum.Core;
using CheckSum.Core.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CheckSum.Tests
{
    /// <summary>
    ///     Тесты для ядра.
    /// </summary>
    [TestClass]
    public class CoreTests
    {
        private const string existFolderPath = @"..\..\TestFiles";
        private const string notExistFolderPath = @"D:\notExistFolder";
        private const string zeroSizeFileName = "ZeroSizeFile.txt";
        private const string size200FileName = "200SizeFile.txt";

        [TestMethod]
        public async Task AnalizeSmokeTest()
        {
            var isDone = false;
            var xmlFileName = string.Empty;
            var progress = new Progress<FileResult>();
            var folderCheckSum = new FolderCheckSum(existFolderPath);

            folderCheckSum.Analized += delegate(object sender, string e)
            {
                isDone = true;
                xmlFileName = e;
            };

            await folderCheckSum.AnalizeAsync(progress);

            Assert.IsTrue(isDone, "Анализ не закончился успешно");
            Assert.IsTrue(folderCheckSum.FolderResult.FilesCollection.Any(), "Результаты проверки пусты");
            Assert.IsTrue(new FileInfo(xmlFileName).Exists);
        }

        /// <summary>
        /// Тест для определения правильности подсчета суммы байт файлов
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task FileSizeTest()
        {
            var zeroSizeFilePath = new FileInfo($"{existFolderPath}{Path.DirectorySeparatorChar}{zeroSizeFileName}").FullName;
            var size200FileSizePath = new FileInfo($"{existFolderPath}{Path.DirectorySeparatorChar}{size200FileName}").FullName;
            const long filesize200 = 200L;

            var folderCheckSum = new FolderCheckSum(existFolderPath);
            var progress = new Progress<FileResult>();

            await folderCheckSum.AnalizeAsync(progress);

            var fileResults = folderCheckSum.FolderResult.FilesCollection;

            var zeroSiseResult = fileResults.FirstOrDefault(r => r.FileName.Equals(zeroSizeFilePath));
            Assert.IsNotNull(zeroSiseResult,"Файл с нулевой суммой не найден в результатах");
            Assert.AreEqual(FileAnalyzeStatus.ZeroFile,zeroSiseResult.Status,"Сумма байт файла с нулевым размером не равна нулю");

            var size200Result= fileResults.FirstOrDefault(r => r.FileName.Equals(size200FileSizePath)) as FileResultSuccess ;
            Assert.IsNotNull(size200Result,"");
            Assert.AreEqual(filesize200, size200Result.CheckSum,$"Суммма байт файла не равна {filesize200}");
        }

        /// <summary>
        ///     Определяет, для всех ли файлов сгенерирован прогресс
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void ProgressTest()
        {
            var folderCheckSum = new FolderCheckSum(existFolderPath);
            var progress = new Progress<FileResult>();

            var progressCollection = new List<FileResult>();
            progress.ProgressChanged += delegate(object sender, FileResult e) { progressCollection.Add(e); };

            folderCheckSum.AnalizeAsync(progress).Wait();

            Assert.AreEqual(folderCheckSum.FolderResult.FilesCollection.Count, progressCollection.Count,
                "Количество отловленных событий изменения прогресса не соответствует числу проверенных файлов");
        }

        /// <summary>
        ///     Определяет ошибки анализа файлов
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AnalizeErrorsTest()
        {
            var folderCheckSum = new FolderCheckSum(existFolderPath);
            var progress = new Progress<FileResult>();

            await folderCheckSum.AnalizeAsync(progress);

            Assert.IsTrue(
                folderCheckSum.FolderResult.FilesCollection.All(r => r.Status!=FileAnalyzeStatus.Error),
                "Не все файлы прверены успешно");
        }

        /// <summary>
        ///     Правильно определяет файл с нулевым размером
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AnalizeZeroFileSizeTest()
        {
            var folderCheckSum = new FolderCheckSum(existFolderPath);
            var progress = new Progress<FileResult>();

            await folderCheckSum.AnalizeAsync(progress);

            Assert.IsTrue(
                folderCheckSum.FolderResult.FilesCollection.Any(r => r.Status.Equals(FileAnalyzeStatus.ZeroFile)),
                "Файл с нулевым размером не определен");
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException), "Не поймано исключение об отсутствующей папке")]
        public async Task OpenNotExistFolderTest()
        {
            var folderCheckSum = new FolderCheckSum(notExistFolderPath);
            var progress = new Progress<FileResult>();
            await folderCheckSum.AnalizeAsync(progress);
        }

        [TestMethod]
        public void FolderResultSerializationTest()
        {
            var folderResult = new FolderResult();
            folderResult.FilesCollection.Add(new FileResultSuccess {CheckSum = 1111, FileName = "file1"});
            folderResult.FilesCollection.Add(new FileResultSuccess {CheckSum = 222, FileName = "file2"});
            folderResult.FilesCollection.Add(new FileResultError {FileName = "file3", ErrorMessage = "Error"});

            var fileName = $"{existFolderPath}{Path.DirectorySeparatorChar}testxml.xml";

            try
            {
                folderResult.Save(fileName);
                Assert.IsTrue(new FileInfo(fileName).Exists);
            }
            catch
            {
                Assert.Fail("Произошла ошибка сериализации");
            }

            try
            {
                var deserialisedFolderResult = FolderResult.Open(fileName);
                Assert.AreEqual(folderResult.FilesCollection.Count, deserialisedFolderResult.FilesCollection.Count);
            }
            catch
            {
                Assert.Fail("Произошла ошибка десериализации");
            }
        }
    }
}