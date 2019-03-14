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
                folderCheckSum.FolderResult.FilesCollection.All(r => r.Status.Equals(FileAnalyzeStatus.Success)),
                "Не все файлы прверены успешно");
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