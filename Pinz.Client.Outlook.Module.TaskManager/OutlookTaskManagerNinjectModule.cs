using AutoMapper;
using Com.Pinz.Client.Outlook.Module.TaskManager.Models;
using Com.Pinz.Client.Outlook.Module.TaskManager.Views;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject.Activation;
using Ninject.Modules;

namespace Com.Pinz.Client.Outlook.Module.TaskManager
{
    public class OutlookTaskManagerNinjectModule : NinjectModule
    {
        private readonly static string OUTLOOK_MODEL_NAMED = "OutlookModel";

        public override void Load()
        {
            Kernel.Bind<object>().To<OutlookCategoryListView>().Named("OutlookCategoryListView");

            Kernel.Bind<CategoryListModel>().ToSelf().Named(OUTLOOK_MODEL_NAMED);
            Kernel.Bind<CategoryShowEditModel>().ToSelf().Named(OUTLOOK_MODEL_NAMED);
            Kernel.Bind<TaskEditModel>().ToSelf().Named(OUTLOOK_MODEL_NAMED);
            Kernel.Bind<TaskListModel>().ToSelf().Named(OUTLOOK_MODEL_NAMED);
            Kernel.Bind<TaskShowEditModel>().ToSelf().Named(OUTLOOK_MODEL_NAMED);

            Kernel.Bind<IMapper>().ToMethod(StartAutoMapper).InSingletonScope().Named("OutlookClientMapper");
        }

        private IMapper StartAutoMapper(IContext arg)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OutlookTask,OutlookTask>();
            });

            return config.CreateMapper();
        }
    }
}
