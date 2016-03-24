namespace PinzOutlookAddIn
{
    partial class MainAddInRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MainAddInRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für Designerunterstützung -
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.groupStart = this.Factory.CreateRibbonGroup();
            this.showButton = this.Factory.CreateRibbonButton();
            this.groupFilter = this.Factory.CreateRibbonGroup();
            this.checkBoxFinished = this.Factory.CreateRibbonCheckBox();
            this.checkBoxDueToday = this.Factory.CreateRibbonCheckBox();
            this.checkBoxOnlyMy = this.Factory.CreateRibbonCheckBox();
            this.checkBoxNotStarted = this.Factory.CreateRibbonCheckBox();
            this.checkBoxInProgress = this.Factory.CreateRibbonCheckBox();
            this.tab1.SuspendLayout();
            this.groupStart.SuspendLayout();
            this.groupFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.groupStart);
            this.tab1.Groups.Add(this.groupFilter);
            this.tab1.Label = "Pinz";
            this.tab1.Name = "tab1";
            // 
            // groupStart
            // 
            this.groupStart.Items.Add(this.showButton);
            this.groupStart.Label = "Start";
            this.groupStart.Name = "groupStart";
            // 
            // showButton
            // 
            this.showButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.showButton.Image = global::PinzOutlookAddIn.Properties.Resources.eye_icon;
            this.showButton.Label = "Show";
            this.showButton.Name = "showButton";
            this.showButton.ShowImage = true;
            this.showButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.showButton_Click);
            // 
            // groupFilter
            // 
            this.groupFilter.Items.Add(this.checkBoxFinished);
            this.groupFilter.Items.Add(this.checkBoxDueToday);
            this.groupFilter.Items.Add(this.checkBoxOnlyMy);
            this.groupFilter.Items.Add(this.checkBoxNotStarted);
            this.groupFilter.Items.Add(this.checkBoxInProgress);
            this.groupFilter.Label = "Filter";
            this.groupFilter.Name = "groupFilter";
            // 
            // checkBoxFinished
            // 
            this.checkBoxFinished.Label = "Show finished";
            this.checkBoxFinished.Name = "checkBoxFinished";
            this.checkBoxFinished.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.checkBoxFinished_Click);
            // 
            // checkBoxDueToday
            // 
            this.checkBoxDueToday.Label = "Due today";
            this.checkBoxDueToday.Name = "checkBoxDueToday";
            this.checkBoxDueToday.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.checkBoxDueToday_Click);
            // 
            // checkBoxOnlyMy
            // 
            this.checkBoxOnlyMy.Checked = true;
            this.checkBoxOnlyMy.Enabled = false;
            this.checkBoxOnlyMy.Label = "My Tasks";
            this.checkBoxOnlyMy.Name = "checkBoxOnlyMy";
            // 
            // checkBoxNotStarted
            // 
            this.checkBoxNotStarted.Label = "Not started";
            this.checkBoxNotStarted.Name = "checkBoxNotStarted";
            this.checkBoxNotStarted.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.checkBoxNotStarted_Click);
            // 
            // checkBoxInProgress
            // 
            this.checkBoxInProgress.Label = "In progress";
            this.checkBoxInProgress.Name = "checkBoxInProgress";
            this.checkBoxInProgress.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.checkBoxInProgress_Click);
            // 
            // MainAddInRibbon
            // 
            this.Name = "MainAddInRibbon";
            this.RibbonType = "Microsoft.Outlook.Explorer";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.MainAddInRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.groupStart.ResumeLayout(false);
            this.groupStart.PerformLayout();
            this.groupFilter.ResumeLayout(false);
            this.groupFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupStart;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton showButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupFilter;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox checkBoxFinished;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox checkBoxDueToday;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox checkBoxOnlyMy;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox checkBoxNotStarted;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox checkBoxInProgress;
    }

    partial class ThisRibbonCollection
    {
        internal MainAddInRibbon MainAddInRibbon
        {
            get { return this.GetRibbon<MainAddInRibbon>(); }
        }
    }
}
