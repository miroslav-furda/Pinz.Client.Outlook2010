using Com.Pinz.WpfClient.Module.TaskManager;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Ninject;

namespace Pinz.Client.Outlook.MainPage
{
    public class MainPageModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;
        private readonly ModuleCatalog moduleCatalog;

        [Inject]
        public MainPageModule(IRegionViewRegistry registry, ModuleCatalog moduleCatalog)
        {
            this.regionViewRegistry = registry;
            this.moduleCatalog = moduleCatalog;
        }

        public void Initialize()
        {
            regionViewRegistry.RegisterViewWithRegion("PinzMainPageRegion", typeof(View.MainPageView));
            
        }
    }
}
