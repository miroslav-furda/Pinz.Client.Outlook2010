using Com.Pinz.Client.DomainModel.Service;
using Com.Pinz.Client.ServiceConsumer;
using Ninject;
using Ninject.Modules;
using Pinz.Client.Outlook2010.Service.Orchestration;

namespace Pinz.Client.Outlook2010.Service
{
    public class ServiceNinjectModule : NinjectModule
    {
        public override void Load()
        {
            //Kernel.Load(new ServiceConsumerNinjectModule());

            Kernel.Bind<ITaskWpfService>().To<TaskOrchestratingService>().InSingletonScope();
        }
    }
}
