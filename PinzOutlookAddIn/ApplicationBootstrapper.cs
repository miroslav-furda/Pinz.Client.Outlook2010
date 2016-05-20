﻿using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Tools.Outlook;
using Microsoft.Office.Tools;
using System.Windows;
using Ninject;
using OutlookInterop = Microsoft.Office.Interop.Outlook;
using System.Windows.Markup;
using System.Globalization;
using PinzOutlookAddIn.Ribbon;
using PinzOutlookAddIn.Infrastructure;
using Prism.Ninject;
using Prism.Modularity;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.Client.Module.Main;
using Com.Pinz.Client.Module.Login;
using Com.Pinz.Client.Module.TaskManager;
using Com.Pinz.Client.Module.Administration;
using Com.Pinz.Client.Model;
using AutoMapper;
using Ninject.Activation;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer;
using System;
using Common.Logging;

namespace PinzOutlookAddIn
{
    public class ApplicationBootstrapper : NinjectBootstrapper
    {
        private static readonly ILog Log = LogManager.GetLogger<ApplicationBootstrapper>();

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
            EnsureApplicationResources();
            return this.Kernel.Get<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            //set current culture
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            mainRibbon.setController(this.Kernel.Get<IRibbonController>());

            CustomTaskPane mainTaskPane = this.Kernel.Get<CustomTaskPane>();
            mainTaskPane.Visible = true;
            Application app = Application.Current;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(MainModule));
            moduleCatalog.AddModule(typeof(LoginModule));
            moduleCatalog.AddModule(typeof(TaskManagerModule));
            moduleCatalog.AddModule(typeof(AdministrationModule));

            moduleCatalog.Load();
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            Kernel.Bind<IRibbonController>().To<DefaultRibbonController>();
            Kernel.Bind<Shell>().ToSelf().InSingletonScope();

            //Kernel.Bind<Service.DAO.TaskAndCategoryLoader>().ToSelf().InSingletonScope();
            //Kernel.Bind<ITaskDAO>().To<Service.DAO.OutlookTaskDAO>().InSingletonScope();
            //Kernel.Bind<ICategoryDAO>().To<Service.DAO.OutlookCategoryDAO>().InSingletonScope();

            Kernel.Bind<OutlookInterop.Application>().ToConstant(application);
            Kernel.Bind<CustomTaskPaneCollection>().ToConstant(customTaskPaneCollection);
            Kernel.Bind<RibbonCollectionBase>().ToConstant(thisRibbonCollection);
            Kernel.Bind<FormRegionCollectionBase>().ToConstant(thisFormRegionCollection);
            Kernel.Bind<CustomTaskPane>().ToMethod(context => createMainTaskPane()).InSingletonScope();

            Kernel.Load(new MainNinjectModule());
            Kernel.Load(new AdministrationNinjectModule());
            Kernel.Load(new TaskManagerNinjectModule());
            Kernel.Load(new ServiceConsumerNinjectModule());
            Kernel.Load(new LoginNinjectModule());

            Kernel.Bind<TaskFilter>().ToSelf().InSingletonScope();
            Kernel.Bind<ApplicationGlobalModel>().ToSelf().InSingletonScope();

            Kernel.Bind<IMapper>().ToMethod(StartAutoMapper).InSingletonScope().Named("WpfClientMapper");
        }

        private IMapper StartAutoMapper(IContext arg)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Task, Task>();
                cfg.CreateMap<User, User>();
            });

            return config.CreateMapper();
        }

        private CustomTaskPane createMainTaskPane()
        {
            WpfControlHost controllHost = new WpfControlHost();
            controllHost.WpfElementHost.HostContainer.Children.Add((UIElement)this.Kernel.Get<Shell>());
            CustomTaskPane mainTaskPane = customTaskPaneCollection.Add(controllHost, Properties.Resources.mainTaskPane_title);
            mainTaskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight;
            mainTaskPane.Width = 400;

            return mainTaskPane;
        }

        private void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                // create the Application object
                new Application();

                try
                {
                    // merge in your application resources
                    Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
                             new Uri("PinzOutlookAddIn;component/Themes/Office2010Blue/Office2010Blue.MSControls.Core.Implicit.xaml", UriKind.Relative)) as ResourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
                            new Uri("PinzOutlookAddIn;component/Themes/Office2010Blue/Office2010Blue.MSControls.Toolkit.Implicit.xaml", UriKind.Relative)) as ResourceDictionary);

                    ResourceDictionary rd = Application.LoadComponent(
                            new Uri("Pinz.Client.Commons;component/Themes/PinzResourceDictionary.xaml", UriKind.Relative)) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Add(rd);
                }
                catch (Exception ex)
                {
                    Log.Error("Fialed to load application resources!", ex);
                }
            }
        }

        internal void Shutdown()
        {
            this.Kernel.Dispose();
        }
    }
}
