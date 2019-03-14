using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheckSum.Core;
using CheckSum.Core.Results;
using CheckSum.WpfDemo.Annotations;
using CheckSum.WpfDemo.Commands;
using Application = System.Windows.Application;

namespace CheckSum.WpfDemo
{
    /// <summary>
    ///     Вью модель для FolderCheckSum
    /// </summary>
    public class DirectoryViewModel : INotifyPropertyChanged
    {
        private string analizeState = "Нажмите ... затем Начать анализ";
        private FolderCheckSum folderCheckSum;
        private string folderPath;

        public DirectoryViewModel()
        {
            AnalizedFiles = new ObservableCollection<FileResult>();

            OpenDirectoryCommand = new SyncCommand(OpenDirectory);
            AnalizeCommand = new AsyncCommand(async () => await AnalizeDirectory());
        }

        /// <summary>
        ///     Строка состояния
        /// </summary>
        public string AnalizeState
        {
            get => analizeState;
            set
            {
                if (value == analizeState) return;
                analizeState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Путь для папки
        /// </summary>
        public string FolderPath
        {
            get => folderPath;
            set
            {
                if (value == folderPath) return;
                folderPath = value;
                OnPropertyChanged();
            }
        }

        
        /// <summary>
        ///     Проанализированные файлы
        /// </summary>
        public ObservableCollection<FileResult> AnalizedFiles { get; }

        public async Task AnalizeDirectory()
        {
            try
            {
                folderCheckSum = new FolderCheckSum(FolderPath);

                AnalizeState = "Производит анализ папки. Ждите...";

                folderCheckSum.Analized += FolderCheckSum_Analized;

                var progress = new Progress<FileResult>();
                progress.ProgressChanged += Progress_ProgressChanged;


                await folderCheckSum.AnalizeAsync(progress);
            }
            //Здесь, безусловно нужно сделать нормальную обработку ошибок
            catch (Exception ex)
            {
                AnalizeState = ex.Message;
            }
        }

        private void OpenDirectory(object obj)
        {
            var openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = openFolderDialog.SelectedPath;
            }
        }

        #region Обработка событий

        private void FolderCheckSum_Analized(object sender, string e)
        {
            AnalizeState = $"Анализ папки {e} завершен";
        }

        private void Progress_ProgressChanged(object sender, FileResult e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { AnalizedFiles.Add(e); }));
        }

        #endregion

        #region Команды

        public SyncCommand OpenDirectoryCommand { get; }

        public AsyncCommand AnalizeCommand { get; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}