using Com.Pinz.Client.Outlook.Service.Model;

namespace Com.Pinz.Client.Outlook.Service.DAO
{
    public interface TaskEvents_Event
    {
        event TaskEvents_TaskAddEventHandler TaskAdd;
        event TaskEvents_TaskChangeEventHandler TaskChange;
        event TaskEvents_TaskRemoveEventHandler TaskRemove;
    }

    public delegate void TaskEvents_TaskAddEventHandler(OutlookTask task);
    public delegate void TaskEvents_TaskChangeEventHandler(OutlookTask task);
    public delegate void TaskEvents_TaskRemoveEventHandler(OutlookTask task);
}
