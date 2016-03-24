using Com.Pinz.Client.DomainModel.Model;

namespace Pinz.Client.Outlook2010.Service.OutlookService
{
    public interface TaskEvents_Event
    {
        event TaskEvents_TaskAddEventHandler TaskAdd;
        event TaskEvents_TaskChangeEventHandler TaskChange;
        event TaskEvents_TaskRemoveEventHandler TaskRemove;
    }

    public delegate void TaskEvents_TaskAddEventHandler(Task task, Category category);
    public delegate void TaskEvents_TaskChangeEventHandler(Task task);
    public delegate void TaskEvents_TaskRemoveEventHandler(Task task, Category category);
}
