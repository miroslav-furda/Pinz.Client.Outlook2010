using Com.Pinz.Client.Outlook.Module.TaskManager.Models;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject;
using Prism.Regions;
using System.Windows.Controls;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for TaskEditView.xaml
    /// </summary>
    public partial class TaskEditView : UserControl
    {
        [Inject]
        public TaskEditView([Named("OutlookModel")] TaskEditModel model)
        {
            InitializeComponent();
            this.DataContext = model;

            RegionContext.GetObservableContext(this).PropertyChanged += (s, e) =>
            {
                if (RegionContext.GetObservableContext(this).Value != null)
                    model.Task = RegionContext.GetObservableContext(this).Value as OutlookTask;
            };
        }
    }
}
