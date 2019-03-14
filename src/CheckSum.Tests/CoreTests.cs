using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CheckSum.Core;
using CheckSum.Core.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CheckSum.Tests
{
    [TestClass]
    public class CoreTests
    {
        private const string existFolderPath = @"..\..\TestFiles";
        private const string notExistFolderPath = @"D:\notExistFolder";

        [TestMethod]
        public async Task OpenExistFolderTest()
        {
            var folderCheckSum = new FolderCheckSum(existFolderPath);
            var progress= new Progress<FileResult>();
            await folderCheckSum.AnalizeAsync(progress);

            Assert.IsTrue(folderCheckSum.FolderResult.FilesCollection.Any());
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
        public async Task FolderResultSerializationTest()
        {
         
        }
    }
}
