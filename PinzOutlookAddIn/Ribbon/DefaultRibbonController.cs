using Microsoft.Office.Tools;
using Ninject;
using Pinz.Client.Outlook2010.Service.OutlookModel;

namespace PinzOutlookAddIn.Ribbon
{
    public class DefaultRibbonController : IRibbonController
    {
        private CustomTaskPane mainTaskPane;
        private TaskFilter taskFilter;

        [Inject]
        public DefaultRibbonController(CustomTaskPane taskPane, TaskFilter filter)
        {
            this.mainTaskPane = taskPane;
            this.taskFilter = filter;
        }

        public void showDueToday(bool selected)
        {
            taskFilter.DueToday = selected;
        }

        public void showFinished(bool selected)
        {
            taskFilter.Complete = selected;
        }

        public void showInProgress(bool selected)
        {
            taskFilter.InProgress = selected;
        }

        public void showMainTaskPane()
        {
            this.mainTaskPane.Visible = true;
        }

        public void showNotStarted(bool selected)
        {
            taskFilter.NotStarted = selected;
        }
    }
}
