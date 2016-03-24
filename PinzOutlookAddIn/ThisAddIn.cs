using System;
using Microsoft.Office.Tools.Ribbon;

namespace PinzOutlookAddIn
{
    public partial class ThisAddIn
    {
        private ApplicationBootstrapper _bootstrapper;
        private MainAddInRibbon mainRibbon;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Optimisation for better performance
            System.Windows.Forms.Application.Idle += OnIdle;
        }

        private void OnIdle(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle -= OnIdle;

            _bootstrapper = new ApplicationBootstrapper(this.Application, this.CustomTaskPanes, Globals.Ribbons, Globals.FormRegions, mainRibbon);
            _bootstrapper.Run();
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            mainRibbon = new MainAddInRibbon();
            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new IRibbonExtension[] { mainRibbon });
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Hinweis: Outlook löst dieses Ereignis nicht mehr aus. Wenn Code vorhanden ist, der 
            //    ausgeführt werden muss, wenn Outlook geschlossen wird, informieren Sie sich unter http://go.microsoft.com/fwlink/?LinkId=506785
            _bootstrapper.Shutdown();
        }

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
