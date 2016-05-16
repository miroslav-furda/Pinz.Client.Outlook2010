using Com.Pinz.Client.Outlook.Service.Model;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Outlook.Service
{
    public interface ITaskService
    {
        ObservableCollection<OutlookTask> ReadByCategory(OutlookCategory category);
        void StartTask(OutlookTask task);
        void CompleteTask(OutlookTask task);
        void ReopenTask(OutlookTask task);
        void Update(OutlookTask task);
        OutlookTask CreateNewTask(OutlookCategory category);
        void Delete(OutlookTask task);
        void MoveToCategory(OutlookTask sourceItem, OutlookCategory newCategory);
    }
}
