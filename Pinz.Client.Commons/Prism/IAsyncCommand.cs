using System.Threading.Tasks;
using System.Windows.Input;

namespace Com.Pinz.Client.Commons.Prism
{
    public interface IAsyncCommand<in T> : ICommand
    {
        Task ExecuteAsync(T obj);
        ICommand Command { get; }
    }
}
