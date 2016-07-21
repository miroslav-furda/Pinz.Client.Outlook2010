using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.Client.Model;
using Common.Logging;
using Microsoft.Office.Tools;
using Ninject;
using Prism.Regions;
using System;
using System.Windows;

namespace PinzOutlookAddIn.Ribbon
{
    public class DefaultRibbonController : IRibbonController
    {
        private readonly static ILog Log = LogManager.GetLogger<DefaultRibbonController>();

        private CustomTaskPane _mainTaskPane;
        private TaskFilter _taskFilter;
        private IRegionManager _regionManager;
        public ApplicationGlobalModel AppGlobalModel { get; private set; }

        private static Uri AdministrationViewUri = new Uri("AdministrationMainView", UriKind.Relative);
        private static Uri PinzProjectsTabViewUri = new Uri("PinzProjectsTabView", UriKind.Relative);
        private static Uri OutlookCategoryListViewUri = new Uri("OutlookCategoryListView", UriKind.Relative);


        [Inject]
        public DefaultRibbonController(CustomTaskPane taskPane, TaskFilter filter, IRegionManager regionManager,
            ApplicationGlobalModel appGlobalModel)
        {
            this._mainTaskPane = taskPane;
            this._taskFilter = filter;
            this._regionManager = regionManager;
            this.AppGlobalModel = appGlobalModel;
        }

        public void showDueToday(bool selected)
        {
            _taskFilter.DueToday = selected;
        }

        public void showFinished(bool selected)
        {
            _taskFilter.Complete = selected;
        }

        public void showInProgress(bool selected)
        {
            _taskFilter.InProgress = selected;
        }

        public void showMyTasks(bool selected)
        {
            _taskFilter.MyTasks = selected;
        }

        public void showMainTaskPane()
        {
            this._mainTaskPane.Visible = true;
        }

        public void showNotStarted(bool selected)
        {
            _taskFilter.NotStarted = selected;
        }

        public void NavigateToPinzAdministration()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Log.Debug("request navigate");
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, AdministrationViewUri, (res) =>
                {
                    Log.DebugFormat("Navigation result was {0}, with Context: {1}, and error: {2} ", res.Result, res.Context, res.Error);
                });
            }));
        }

        public void NavigateToPinzTasks()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, PinzProjectsTabViewUri, (res) =>
                {
                    Log.DebugFormat("Navigation result was {0}, with Context: {1}, and error: {2} ", res.Result, res.Context, res.Error);
                });
            }));
        }

        public void NavigateToOutlookView()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, OutlookCategoryListViewUri, (res) =>
                {
                    Log.DebugFormat("Navigation result was {0}, with Context: {1}, and error: {2} ", res.Result, res.Context, res.Error);
                });
            }));
        }

        public void NavigateToPinzView()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, PinzProjectsTabViewUri, (res) =>
                {
                    Log.DebugFormat("Navigation result was {0}, with Context: {1}, and error: {2} ", res.Result, res.Context, res.Error);
                });
            }));
        }
    }
}
