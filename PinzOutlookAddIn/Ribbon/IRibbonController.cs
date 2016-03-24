using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinzOutlookAddIn.Ribbon
{
    public interface IRibbonController
    {
        void showMainTaskPane();
        void showFinished(bool finished);
        void showDueToday(bool finished);
        void showNotStarted(bool finished);
        void showInProgress(bool finished);
    }
}
