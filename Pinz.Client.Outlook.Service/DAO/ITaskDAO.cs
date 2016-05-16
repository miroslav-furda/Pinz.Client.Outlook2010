using Com.Pinz.Client.Outlook.Service.Model;
using System.Collections.Generic;

namespace Com.Pinz.Client.Outlook.Service.DAO
{
    public interface ITaskDAO : TaskEvents_Event
    {
        List<OutlookTask> readAll();
        void create(OutlookTask task);
        OutlookTask update(OutlookTask task);
        void delete(OutlookTask task);
    }
}
