using System;
using Microsoft.Office.Tools.Ribbon;
using Common.Logging;
using PinzOutlookAddIn.Infrastructure;
using System.Windows;
using System.Diagnostics;

namespace PinzOutlookAddIn
{
    public partial class ThisAddIn
    {
        private static readonly ILog Log = LogManager.GetLogger<ThisAddIn>();

        private ApplicationBootstrapper _bootstrapper;
        private MainAddInRibbon mainRibbon;
        private ApplicationInsightHelper applicationInsightHelper;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Log.Debug("ThisAddIn_Startup...");
            // Optimisation for better performance
            System.Windows.Forms.Application.Idle += OnIdle;
        }

        private void OnIdle(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle -= OnIdle;

            applicationInsightHelper = new ApplicationInsightHelper();

            System.Windows.Forms.Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _bootstrapper = new ApplicationBootstrapper(this.Application, this.CustomTaskPanes, Globals.Ribbons, Globals.FormRegions, mainRibbon);
            applicationInsightHelper.TrackPageView("ThisAddIn");
            _bootstrapper.Run();
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            mainRibbon = new MainAddInRibbon();
            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new IRibbonExtension[] { mainRibbon });
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            applicationInsightHelper.FlushData();
            System.Windows.Forms.Application.ThreadException -= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            // Hinweis: Outlook löst dieses Ereignis nicht mehr aus. Wenn Code vorhanden ist, der 
            //    ausgeführt werden muss, wenn Outlook geschlossen wird, informieren Sie sich unter http://go.microsoft.com/fwlink/?LinkId=506785
            _bootstrapper.Shutdown();
        }

        #region Error handling
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private void HandleException(Exception exp)
        {
            if (exp is TimeoutException)
            {
                MessageBox.Show(Properties.Resources.Error_Timeout_Content,
                    Properties.Resources.Warning_MessageBox_Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                applicationInsightHelper.TrackFatalException(exp);
                applicationInsightHelper.FlushData();
                Log.ErrorFormat("An unhandled exception just occurred:{0}", exp, exp.Message);

                MessageBox.Show(Properties.Resources.Error_Undefined_Content + exp.Message,
                    Properties.Resources.Error_MessageBox_Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ThisAddIn_Shutdown(null, null);
        }
        #endregion

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
