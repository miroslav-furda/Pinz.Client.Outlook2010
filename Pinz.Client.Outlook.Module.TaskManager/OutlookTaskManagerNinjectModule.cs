using AutoMapper;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject.Activation;
using Ninject.Modules;

namespace Com.Pinz.Client.Outlook.Module.TaskManager
{
    public class OutlookTaskManagerNinjectModule : NinjectModule
    {
        public override void Load()
        {
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
