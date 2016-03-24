using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using Ninject;
using System;
using System.Linq;
using Com.Pinz.Client.DomainModel.Model;
using Pinz.Client.Outlook2010.Service.OutlookModel;
using Com.Pinzonline.DomainModel;

namespace PinzOutlookAddIn.Service
{
    internal class TaskAndCategoryLoader
    {
        private static readonly DateTime MAX_DATE = new DateTime(4501, 1, 1);

        public List<OutlookTask> Tasks { get; private set; }
        public List<OutlookCategory> Categories { get; private set; }
        public OutlookCategory DefaultCategory { get; private set; }


        private Outlook.Items outlookItems;

        [Inject]
        public TaskAndCategoryLoader(Outlook.Application outlookApp)
        {
            Outlook.MAPIFolder outlookTasksFolder = outlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderTasks);
            outlookItems = outlookTasksFolder.Items;
            Tasks = new List<OutlookTask>();
            Categories = new List<OutlookCategory>();

            HashSet<OutlookCategory> cats = new HashSet<OutlookCategory>();
            DefaultCategory = new OutlookCategory() { Name = Properties.Resources.Outlook_Category_Default };
            cats.Add(DefaultCategory);

            foreach (Outlook.TaskItem taskitem in outlookItems)
            {
                if (!String.IsNullOrWhiteSpace(taskitem.Categories) && !cats.Any(c => c.Name == taskitem.Categories))
                {
                    cats.Add(new OutlookCategory() { Name = taskitem.Categories });
                }

                OutlookTask newTask = new OutlookTask();
                UpdateTask(newTask, taskitem, cats, DefaultCategory);
                Tasks.Add(newTask);
            }
            cats.ToList().ForEach(Categories.Add);
        }


        public Task UpdateTask(OutlookTask TargetTask, Outlook.TaskItem SourceTaskItem, ICollection<OutlookCategory> categories, OutlookCategory defaultCategory)
        {
            OutlookCategory category = categories.Where(x => x.Name.Equals(SourceTaskItem.Categories)).SingleOrDefault();
            if (category == null)
            {
                category = defaultCategory;
            }

            TargetTask.EntryId = SourceTaskItem.EntryID;
            TargetTask.TaskName = SourceTaskItem.Subject;
            TargetTask.Body = SourceTaskItem.Body;
            TargetTask.IsComplete = SourceTaskItem.Complete;
            //TargetTask.Owner = SourceTaskItem.Owner;
            TargetTask.CreationTime = SourceTaskItem.CreationTime;
            TargetTask.DateCompleted = (SourceTaskItem.DateCompleted.Year == 4501 ? (DateTime?)null : SourceTaskItem.DateCompleted);
            TargetTask.StartDate = (SourceTaskItem.StartDate.Year == 4501 ? (DateTime?)null : SourceTaskItem.StartDate);
            TargetTask.DueDate = (SourceTaskItem.DueDate.Year == 4501 ? (DateTime?)null : SourceTaskItem.DueDate);
            TargetTask.ActualWork = SourceTaskItem.ActualWork;
            TargetTask.Status = fromOutlookStatus(SourceTaskItem.Status);
            TargetTask.Priority = ToTaskPriority(SourceTaskItem.SchedulePlusPriority);
            TargetTask.Category = category;

            return TargetTask;
        }

        public Outlook.TaskItem UpdateOutlookTaskItem(Outlook.TaskItem targetOutloookTaskItem, OutlookTask sourceTask)
        {
            targetOutloookTaskItem.Subject = sourceTask.TaskName;
            targetOutloookTaskItem.Body = sourceTask.Body;
            targetOutloookTaskItem.Complete = sourceTask.IsComplete;
            //targetOutloookTaskItem.Owner = sourceTask.Owner;
            targetOutloookTaskItem.DateCompleted = sourceTask.DateCompleted ?? MAX_DATE;
            targetOutloookTaskItem.StartDate = sourceTask.StartDate ?? MAX_DATE;
            targetOutloookTaskItem.DueDate = sourceTask.DueDate ?? MAX_DATE;

            targetOutloookTaskItem.ActualWork = sourceTask.ActualWork;

            targetOutloookTaskItem.Status = toOutlookStatus(sourceTask.Status);
            targetOutloookTaskItem.SchedulePlusPriority = FromTaskPriority(sourceTask.Priority);
            targetOutloookTaskItem.Categories = sourceTask.Category.Name;

            return targetOutloookTaskItem;
        }

        private TaskPriority? ToTaskPriority(string strPriority)
        {
            TaskPriority? retVal = null;
            if (!String.IsNullOrEmpty(strPriority))
            {
                switch (strPriority.ToLower())
                {
                    case "low":
                        retVal = TaskPriority.Low;
                        break;
                    case "normal":
                        retVal = TaskPriority.Normal;
                        break;
                    case "high":
                        retVal = TaskPriority.High;
                        break;
                    default:
                        retVal = null;
                        break;
                }
            }
            return retVal;
        }

        private string FromTaskPriority(TaskPriority? priority)
        {
            string retVal = null;
            if (priority != null)
            {
                retVal = priority.ToString().ToLower();
            }
            return retVal;
        }

        private TaskStatus fromOutlookStatus(Outlook.OlTaskStatus status)
        {
            TaskStatus retVal;
            switch (status)
            {
                case Outlook.OlTaskStatus.olTaskNotStarted:
                    retVal = TaskStatus.TaskNotStarted;
                    break;
                case Outlook.OlTaskStatus.olTaskInProgress:
                    retVal = TaskStatus.TaskInProgress;
                    break;
                case Outlook.OlTaskStatus.olTaskComplete:
                    retVal = TaskStatus.TaskComplete;
                    break;
                case Outlook.OlTaskStatus.olTaskWaiting:
                    retVal = TaskStatus.TaskWaiting;
                    break;
                case Outlook.OlTaskStatus.olTaskDeferred:
                    retVal = TaskStatus.TaskDeferred;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return retVal;
        }

        private Outlook.OlTaskStatus toOutlookStatus(TaskStatus status)
        {
            Outlook.OlTaskStatus retVal;
            switch (status)
            {
                case TaskStatus.TaskNotStarted:
                    retVal = Outlook.OlTaskStatus.olTaskNotStarted;
                    break;
                case TaskStatus.TaskInProgress:
                    retVal = Outlook.OlTaskStatus.olTaskInProgress;
                    break;
                case TaskStatus.TaskComplete:
                    retVal = Outlook.OlTaskStatus.olTaskComplete;
                    break;
                case TaskStatus.TaskWaiting:
                    retVal = Outlook.OlTaskStatus.olTaskWaiting;
                    break;
                case TaskStatus.TaskDeferred:
                    retVal = Outlook.OlTaskStatus.olTaskDeferred;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return retVal;
        }
    }
}
