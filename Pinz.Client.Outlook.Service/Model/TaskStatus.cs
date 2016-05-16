
namespace Com.Pinz.Client.Outlook.Service.Model
{
    public enum TaskStatus
    {
        TaskNotStarted,    //The task has not yet started.
        TaskInProgress,    //The task is in progress.
        TaskComplete,  //The task is complete.
        TaskWaiting,   //The task is waiting on someone else.
        TaskDeferred   //The task is deferred.
    }
}
