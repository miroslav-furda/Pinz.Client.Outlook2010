using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Model;
using Microsoft.Office.Tools;
using Ninject;
using Prism.Regions;
using System;

namespace PinzOutlookAddIn.Ribbon
{
    public class DefaultRibbonController : IRibbonController
    {
        private CustomTaskPane mainTaskPane;
        private TaskFilter taskFilter;
        private IRegionManager regionManager;

        private static Uri AdministrationViewUri = new Uri("AdministrationMainView", UriKind.Relative);
        private static Uri PinzProjectsTabViewUri = new Uri("PinzProjectsTabView", UriKind.Relative);

        [Inject]
        public DefaultRibbonController(CustomTaskPane taskPane, TaskFilter filter,  IRegionManager regionManager)
        {
            this.mainTaskPane = taskPane;
            this.taskFilter = filter;
            this.regionManager = regionManager;
        }

        public void showDueToday(bool selected)
        {
            taskFilter.DueToday = selected;
        }

        public void showFinished(bool selected)
        {
            taskFilter.Complete = selected;
        }

        public void showInProgress(bool selected)
        {
            taskFilter.InProgress = selected;
        }

        public void showMainTaskPane()
        {
            this.mainTaskPane.Visible = true;
        }

        public void showNotStarted(bool selected)
        {
            taskFilter.NotStarted = selected;
        }

        public void NavigateToPinzAdministration()
        {
            regionManager.RequestNavigate(RegionNames.MainContentRegion, AdministrationViewUri);
        }

        public void NavigateToPinzTasks()
        {
            regionManager.RequestNavigate(RegionNames.MainContentRegion, PinzProjectsTabViewUri);
        }
    }
}
