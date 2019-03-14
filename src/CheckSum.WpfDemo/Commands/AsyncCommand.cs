using System;
using System.Threading.Tasks;

namespace CheckSum.WpfDemo.Commands
{
    /// <summary>
    /// Я использую куски кода комманд из своего репозитория кода. Так привык.
    /// </summary>
   public class AsyncCommand : AsyncCommandBase
    {
        private readonly Func<Task> command;

        public AsyncCommand(Func<Task> command)
        {
            this.command = command;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override Task ExecuteAsync(object parameter)
        {
            return command();
        }
    }
}