using Com.Pinz.WpfClient.Module.TaskManager.Models;
using Ninject;
using System.Windows.Controls;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for CategoryListView.xaml
    /// </summary>
    public partial class CategoryListView : UserControl
    {
        [Inject]
        public CategoryListView(CategoryListModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
