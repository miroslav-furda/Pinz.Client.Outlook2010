using Ninject;
using Prism.Modularity;
using Prism.Regions;

namespace Com.Pinz.Client.Outlook.Module.TaskManager
{
    public class OutlookTaskManagerModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;

        [Inject]
        public OutlookTaskManagerModule(IRegionViewRegistry registry)
        {
            this.regionViewRegistry = registry;
        }

        public void Initialize()
        {
            //regionViewRegistry.RegisterViewWithRegion("OutlookMainRegion", typeof(Views.OutlookCategoryListView));
            regionViewRegistry.RegisterViewWithRegion("OutlookCategoryShowEditRegion", typeof(Views.CategoryShowEditView));
            regionViewRegistry.RegisterViewWithRegion("OutlookTaskListRegion", typeof(Views.TaskListView));
            regionViewRegistry.RegisterViewWithRegion("OutlookTaskShowEditRegion", typeof(Views.TaskShowEditView));
            regionViewRegistry.RegisterViewWithRegion("OutlookTaskEditRegion", typeof(Views.TaskEditView));
        }
    }
}
