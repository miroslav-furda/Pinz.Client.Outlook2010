﻿using System;
using Microsoft.Office.Tools.Ribbon;
using Prism.Regions;
using Com.Pinz.Client.Commons;
using System.ComponentModel;

namespace PinzOutlookAddIn
{
    public partial class MainAddInRibbon
    {
        private Ribbon.IRibbonController controller;

        private void MainAddInRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        public void setController(Ribbon.IRibbonController ribbonController)
        {
            this.controller = ribbonController;
            adminToggleButton.Enabled = ribbonController.AppGlobalModel.IsUserLoggedIn;
            ribbonController.AppGlobalModel.PropertyChanged += _appGlobalModel_PropertyChanged;

        }

        private void _appGlobalModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsUserLoggedIn" == e.PropertyName)
            {
                adminToggleButton.Enabled = controller.AppGlobalModel.IsUserLoggedIn;
            }
        }

        private void showButton_Click(object sender, RibbonControlEventArgs e)
        {
            this.controller.showMainTaskPane();
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

        private void adminToggleButton_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonToggleButton button = sender as RibbonToggleButton;
            if (button.Checked)
                controller.NavigateToPinzAdministration();
            else
                controller.NavigateToPinzTasks();
        }

        private void checkBoxOnlyMy_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonCheckBox checkBox = (RibbonCheckBox)sender;
            this.controller.showMyTasks(checkBox.Checked);
        }

        private void outlookPinzToggleButton_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonToggleButton button = sender as RibbonToggleButton;
            if (button.Checked)
            {
                button.Label = "Pinz";
                controller.NavigateToPinzView();
            }
            else
            {
                button.Label = "Outlook";
                controller.NavigateToOutlookView();
            }
        }
    }
}
