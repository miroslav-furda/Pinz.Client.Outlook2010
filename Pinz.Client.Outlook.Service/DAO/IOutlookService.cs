using Com.Pinz.Client.Outlook.Service.Model;
using System.Collections.Generic;

namespace Com.Pinz.Client.Outlook.Service.DAO
{
    public interface IOutlookService : TaskEvents_Event
    {
        List<OutlookCategory> ReadAllCategories();

        List<OutlookTask> ReadAllTasksByCategory(OutlookCategory category);

        void ChangeTaskStatus(OutlookTask task, Model.TaskStatus newStatus);

        void MoveTaskToCategory(OutlookTask task, OutlookCategory category);

        #region Category CUD

        OutlookCategory CreateCategory();

        void DeleteCategory(OutlookCategory category);

        void UpdateCategory(OutlookCategory category);
        #endregion

        #region Task CUD

        OutlookTask CreateTaskInCategory(OutlookCategory category);

        void DeleteTask(OutlookTask task);

        void UpdateTask(OutlookTask task);
        #endregion

    }
}