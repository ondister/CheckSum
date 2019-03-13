using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using CheckSum.Core;
using CheckSum.Core.Results;
using CheckSum.WpfDemo.Annotations;
using Application = System.Windows.Application;

namespace CheckSum.WpfDemo
{
   public class DirectoryViewModel:INotifyPropertyChanged
    {
        
        private FolderCheckSum folderCheckSum;
#warning не забыть сделать открытие папки


        private string analizeState="Нажмите выбрать папку. затем начать анализ";
        public ObservableCollection<FileResult> AnalizedFiles { get; }

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

        public DirectoryViewModel()
        {
            AnalizedFiles=new ObservableCollection<FileResult>();
            AnalizeCommand = new AsyncCommand(async () => await AnalizeDirectory());
            OpenDirectoryCommand = new SyncCommand(OpenDirectory);
        }

        private void OpenDirectory(object obj)
        {
            var openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                var dialogPath = openFolderDialog.SelectedPath;
                folderCheckSum = new FolderCheckSum(dialogPath);
            }
            
        }

        public SyncCommand OpenDirectoryCommand { get;  }

        public AsyncCommand AnalizeCommand { get; }

       
        private void FolderCheckSum_Analized(object sender, EventArgs e)
        {
            AnalizeState = "Анализ завершен";
        }

        public async Task AnalizeDirectory()
        {
            if (folderCheckSum != null)
            {
                AnalizeState = "Производит анализ папки. Ждите...";

                folderCheckSum.Analized += FolderCheckSum_Analized;

                var progress= new Progress<FileResult>();
                progress.ProgressChanged += Progress_ProgressChanged;

                try
                {
                    await folderCheckSum.AnalizeAsync(progress);
                }
                catch(Exception ex)
                {
                    AnalizeState = ex.Message;
                }
            }
 
        }

        private void Progress_ProgressChanged(object sender, FileResult e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { AnalizedFiles.Add(e); }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
