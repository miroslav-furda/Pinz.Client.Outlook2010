using Com.Pinz.Client.Outlook.Module.TaskManager.Models;
using Ninject;
using System.Windows.Controls;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for CategoryListView.xaml
    /// </summary>
    public partial class OutlookCategoryListView : UserControl
    {
        [Inject]
        public OutlookCategoryListView([Named("OutlookModel")] CategoryListModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
