using Com.Pinz.Client.Commons.Model;
using Ninject.Modules;

namespace Com.Pinz.Client.Outlook.Service
{
    public class ServiceNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ITaskService>().To<Impl.TaskService>().InSingletonScope();
            Kernel.Bind<ICategoryService>().To<Impl.CategoryService>().InSingletonScope();
            Kernel.Bind<TaskFilter>().ToSelf().InSingletonScope();
        }
    }
}
