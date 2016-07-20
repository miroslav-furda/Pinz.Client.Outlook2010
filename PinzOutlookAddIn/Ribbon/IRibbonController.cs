using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinzOutlookAddIn.Ribbon
{
    public interface IRibbonController
    {
        void NavigateToPinzAdministration();
        void NavigateToPinzTasks();

        void showMyTasks(bool selected);
        void showMainTaskPane();
        void showFinished(bool finished);
        void showDueToday(bool finished);
        void showNotStarted(bool finished);
        void showInProgress(bool finished);
        void NavigateToOutlookView();
        void NavigateToPinzView();
    }
}
