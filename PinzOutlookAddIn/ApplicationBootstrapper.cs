using Microsoft.Practices.Prism.Modularity;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Tools.Outlook;
using Microsoft.Office.Tools;
using System.Windows;
using Ninject;
using OutlookInterop = Microsoft.Office.Interop.Outlook;
using System.Windows.Markup;
using System.Globalization;
using Com.Pinz.Commons.Client.Prism.NinjectExtension;
using PinzOutlookAddIn.Ribbon;
using PinzOutlookAddIn.Infrastructure;
using Pinz.Client.Outlook2010.Service;
using Pinz.Client.Outlook2010.Service.OutlookService;
using Pinz.Client.Outlook2010.Service.OutlookModel;
using Pinz.Client.Outlook.MainPage;
using Com.Pinz.WpfClient.Module.TaskManager;
using Com.Pinz.Client.ServiceConsumer;

namespace PinzOutlookAddIn
{
    public class ApplicationBootstrapper : NinjectBootstrapper
    {
        private OutlookInterop.Application application;
        private CustomTaskPaneCollection customTaskPaneCollection;
        private RibbonCollectionBase thisRibbonCollection;
        private FormRegionCollectionBase thisFormRegionCollection;
        private MainAddInRibbon mainRibbon;

        public ApplicationBootstrapper(OutlookInterop.Application application, CustomTaskPaneCollection customTaskPaneCollection,
            RibbonCollectionBase thisRibbonCollection, FormRegionCollectionBase thisFormRegionCollection, MainAddInRibbon mainRibbon)
        {
            this.application = application;
            this.customTaskPaneCollection = customTaskPaneCollection;
            this.thisRibbonCollection = thisRibbonCollection;
            this.thisFormRegionCollection = thisFormRegionCollection;
            this.mainRibbon = mainRibbon;
        }

        protected override DependencyObject CreateShell()
        {
            return this.Kernel.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            //set current culture
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            mainRibbon.setController(this.Kernel.Resolve<IRibbonController>());

            CustomTaskPane mainTaskPane = this.Kernel.Resolve<CustomTaskPane>();
            mainTaskPane.Visible = true;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(TaskManagerModule));
            moduleCatalog.AddModule(typeof(MainPageModule));
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            this.Kernel.Load(new ServiceConsumerNinjectModule());
            this.Kernel.Load(new ServiceNinjectModule());
            this.Kernel.Load(new MainPageNinjectModule());

            Kernel.Bind<TaskFilter>().ToSelf().InSingletonScope();
            this.Kernel.Bind<IRibbonController>().To<DefaultRibbonController>();
            this.Kernel.Bind<Shell>().ToSelf();

            Kernel.Bind<Service.TaskAndCategoryLoader>().ToSelf().InSingletonScope();
            Kernel.Bind<ITaskOutlookService>().To<Service.TaskOutlookServiceImpl>().InSingletonScope();

            this.Kernel.Bind<OutlookInterop.Application>().ToConstant(application);
            this.Kernel.Bind<CustomTaskPaneCollection>().ToConstant(customTaskPaneCollection);
            this.Kernel.Bind<RibbonCollectionBase>().ToConstant(thisRibbonCollection);
            this.Kernel.Bind<FormRegionCollectionBase>().ToConstant(thisFormRegionCollection);
            this.Kernel.Bind<CustomTaskPane>().ToMethod(context => createMainTaskPane()).InSingletonScope();

        }

        private CustomTaskPane createMainTaskPane()
        {
            WpfControlHost controllHost = new WpfControlHost();
            controllHost.WpfElementHost.HostContainer.Children.Add((UIElement)this.Kernel.Resolve<Shell>());
            CustomTaskPane mainTaskPane = customTaskPaneCollection.Add(controllHost, Properties.Resources.mainTaskPane_title);
            mainTaskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight;
            mainTaskPane.Width = 400;

            return mainTaskPane;
        }

        internal void Shutdown()
        {
            this.Kernel.Dispose();
        }
    }
}
