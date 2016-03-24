using Com.Pinz.WpfClient.Module.TaskManager;
using Ninject;
using Ninject.Modules;

namespace Pinz.Client.Outlook.MainPage
{
    public class MainPageNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Load(new TaskManagerNinjectModule());
        }
    }
}
