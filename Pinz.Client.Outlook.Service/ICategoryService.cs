using Com.Pinz.Client.Outlook.Service.Model;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Outlook.Service
{
    public interface ICategoryService
    {
        ObservableCollection<OutlookCategory> readAll();
        void create(OutlookCategory category);
        void update(OutlookCategory category);
    }
}
