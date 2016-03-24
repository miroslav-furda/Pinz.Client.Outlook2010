using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace PinzOutlookAddIn
{
    public partial class MainAddInRibbon
    {
        private Ribbon.IRibbonController controller;

        private void MainAddInRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void showButton_Click(object sender, RibbonControlEventArgs e)
        {
            this.controller.showMainTaskPane();
        }

        public void setController(Ribbon.IRibbonController ribbonController)
        {
            this.controller = ribbonController;
        }

        private void checkBoxDueToday_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonCheckBox checkBox = (RibbonCheckBox)sender;
            this.controller.showDueToday(checkBox.Checked);
        }

        private void checkBoxFinished_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonCheckBox checkBox = (RibbonCheckBox)sender;
            this.controller.showFinished(checkBox.Checked);
        }

        private void checkBoxNotStarted_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonCheckBox checkBox = (RibbonCheckBox)sender;
            this.controller.showNotStarted(checkBox.Checked);
        }

        private void checkBoxInProgress_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonCheckBox checkBox = (RibbonCheckBox)sender;
            this.controller.showInProgress(checkBox.Checked);
        }
    }
}
