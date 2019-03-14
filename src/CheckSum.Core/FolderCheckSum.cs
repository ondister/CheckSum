using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using CheckSum.Core.Results;

namespace CheckSum.Core
{
    /// <summary>
    ///     Класс, имплементирующий папку, в которой можно подсчитать сумму значений байт файлов
    ///     Основная проблема - не известно насколько глубока иерархия папок.
    ///     Поэтому используем паттерн produser-consumer.
    ///     Один поток начинает проходить по дереву каталогов, и помещать инфо о файлах в коллекцию
    ///     В то же время асинхронно начинается чтение инфо из коллекции и создание потока для анализа каждого файла.
    /// </summary>
    public class FolderCheckSum
    {
        private const string directoryNotFoundMessage = "Не существует указанной папки";
        private const string folderResultFileName = "FolderCheckSum.xml";

        private readonly BlockingCollection<FileInfo> fileInfos;
        private readonly object lockObject = new object();
        private readonly DirectoryInfo rootDirectoryInfo;

        /// <summary>
        ///     Итоговый результат анализа
        /// </summary>
        public FolderResult FolderResult { get; }

        /// <summary>
        ///     Событие о том, что все файлы проверены
        /// </summary>
        public event EventHandler<string> Analized;

        private Task GetFileInfos(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException($"{directoryNotFoundMessage}: {directoryInfo.FullName}");
            }

            return Task.Run(async () =>
            {
                foreach (var fileInfo in directoryInfo.EnumerateFiles())
                {
                    //Костыль, по сути для перехвата слишком длинного пути. Нужно хорошо подумать, что с этим делать
                    if (!PathTooLong(fileInfo))
                    {
                        fileInfos.Add(fileInfo);
                    }
                }

                foreach (var directory in directoryInfo.EnumerateDirectories())
                {
                    await GetFileInfos(directory);
                }
            });
        }

        private static bool PathTooLong(FileSystemInfo fileInfo)
        {
            try
            {
              var fileName=  fileInfo.FullName;
                return false;
            }
            catch(PathTooLongException)
            {
                return true;
            }

        }

        /// <summary>
        ///     Запускает анализ папки и подсчитывает сумму значений байт каждого файла
        /// </summary>
        /// <param name="analyzeProgress">Прогресс, в который будет записываться новый проанализированный файл</param>
        /// <returns></returns>
        public async Task AnalizeAsync(IProgress<FileResult> analyzeProgress)
        {
            try
            {
                //запускаем проход по папкам и закрываем коллекцию, когда задача завершится
                GetFileInfos(rootDirectoryInfo).GetAwaiter().OnCompleted(() => fileInfos.CompleteAdding());

                var tasks = new List<Task<FileResult>>();
                while (!fileInfos.IsCompleted)
                {
                    if (fileInfos.TryTake(out var fileInfo))
                    {
                        var task = Task.Run(() =>
                        {
                            var analizer = new FileStreamAnalyser();
                            var fileResult = analizer.AnalyzeFile(fileInfo);

                            //сздаем критическую секцию для записи результата
                            lock (lockObject)
                            {
                                FolderResult.FilesCollection.Add(fileResult);
                            }

                            return fileResult;
                        });

                        tasks.Add(task);

                        //ждем завершенную задачу и удаляем ее из списка задач
                        var endedTask = await Task.WhenAny(tasks.ToArray()).ConfigureAwait(false);
                        tasks.Remove(endedTask);

                        analyzeProgress.Report(endedTask.Result);
                    }
                }
                
                var resultFullFileName =
                    $"{rootDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{folderResultFileName}";

                FolderResult.Save(resultFullFileName);

                OnAnalized(resultFullFileName);
            }
            //Обработка ошибок не реализована в полной мере, так как я не знаю, какого паттерна обработки придерживаться.
            //Об это в задании ничего не написано
            catch (DirectoryNotFoundException ex)
            {
                throw ex;
            }
            catch (SecurityException ex)
            {
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
        }
        
        #region Constructors

        private FolderCheckSum(DirectoryInfo rootDirectoryInfo)
        {
            FolderResult = new FolderResult();
            fileInfos = new BlockingCollection<FileInfo>();
            this.rootDirectoryInfo = rootDirectoryInfo;
        }

        public FolderCheckSum(string directoryPath) : this(new DirectoryInfo(directoryPath))
        {
        }

        #endregion

        protected virtual void OnAnalized(string e)
        {
            Analized?.Invoke(this, e);
        }
    }
}