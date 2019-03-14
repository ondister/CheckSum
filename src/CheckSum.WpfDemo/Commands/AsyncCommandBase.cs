using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CheckSum.WpfDemo.Commands
{
    /// <summary>
    ///     Я использую куски кода комманд из своего репозитория кода. Так привык.
    /// </summary>
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteAsync(object parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}