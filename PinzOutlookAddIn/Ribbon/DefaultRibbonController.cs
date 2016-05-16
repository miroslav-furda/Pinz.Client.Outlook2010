using Com.Pinz.Client.Commons.Model;
using Microsoft.Office.Tools;
using Ninject;

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
