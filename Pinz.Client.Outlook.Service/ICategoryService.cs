using Com.Pinz.Client.Outlook.Service.Model;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Outlook.Service
{
    public interface ICategoryService
    {
        ObservableCollection<OutlookCategory> ReadAllCategories();
        void Create();
        void Update(OutlookCategory category);
    }
}
