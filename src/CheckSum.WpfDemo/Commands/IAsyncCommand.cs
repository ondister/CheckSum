using System.Threading.Tasks;
using System.Windows.Input;

namespace CheckSum.WpfDemo.Commands
{
    /// <summary>
    /// Я использую куски кода комманд из своего репозитория кода. Так привык.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
