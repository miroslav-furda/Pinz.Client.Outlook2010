using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Outlook = Microsoft.Office.Interop.Outlook;
using Com.Pinz.Client.Outlook.Service.DAO;
using Com.Pinz.Client.Outlook.Service.Model;

namespace PinzOutlookAddIn.Service
{
    internal class TaskOutlookServiceImpl
    {
        public event TaskEvents_TaskAddEventHandler TaskAdd;
        public event TaskEvents_TaskChangeEventHandler TaskChange;
        public event TaskEvents_TaskRemoveEventHandler TaskRemove;


        private TaskAndCategoryLoader taskAndCategoryLoader;
        private Outlook.Application outlookApp;
        private Outlook.Items outlookItems;

        [Inject]
        public TaskOutlookServiceImpl(Outlook.Application outlookApp, TaskAndCategoryLoader taskAndCategoryLoader)
        {
            this.outlookApp = outlookApp;
            this.taskAndCategoryLoader = taskAndCategoryLoader;

            Outlook.MAPIFolder outlookTasksFolder = outlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderTasks);
            outlookItems = outlookTasksFolder.Items;

            outlookItems.ItemAdd += OutlookItems_ItemAdd;
            outlookItems.ItemRemove += OutlookItems_ItemRemove;
            outlookItems.ItemChange += OutlookItems_ItemChange;
        }

        #region Read

        public List<OutlookCategory> ReadAllCategories()
        {
            return taskAndCategoryLoader.Categories;
        }

        public List<OutlookTask> ReadAllTasksByCategory(OutlookCategory category)
        {
            List<OutlookTask> tasksInCategory = new List<OutlookTask>();
            taskAndCategoryLoader.Tasks.ForEach(item =>
            {
                if (category.Equals(item.Category))
                    tasksInCategory.Add(item);
            });
            return tasksInCategory;
        }

        #endregion

        public void ChangeTaskStatus(OutlookTask task, TaskStatus newStatus)
        {
            switch (newStatus)
            {
                case TaskStatus.TaskInProgress:
                    task.Status = TaskStatus.TaskInProgress;
                    task.StartDate = DateTime.Today;
                    task.DueDate = DateTime.Today;
                    task.DateCompleted = null;
                    break;
                case TaskStatus.TaskComplete:
                    task.Status = TaskStatus.TaskComplete;
                    task.DateCompleted = DateTime.Today;
                    break;
                case TaskStatus.TaskNotStarted:
                    task.Status = TaskStatus.TaskNotStarted;
                    task.StartDate = null;
                    task.DueDate = null;
                    task.DateCompleted = null;
                    break;
                default:
                    task.Status = newStatus;
                    break;
            }
            UpdateTask(task);
        }

        public void MoveTaskToCategory(OutlookTask task, OutlookCategory category)
        {
            OutlookTask outlookTask = task as OutlookTask;
            if (outlookTask.Category != category)
            {
                outlookTask.Category = category as OutlookCategory;
                UpdateTask(outlookTask);
            }
        }

        #region Category CUD

        public OutlookCategory CreateCategory()
        {
            OutlookCategory category = new OutlookCategory()
            {
                Name = Properties.Resources.Outlook_Category_New
            };
            taskAndCategoryLoader.Categories.Add(category);
            return category;
        }

        public void DeleteCategory(OutlookCategory category)
        {
            throw new NotImplementedException();
        }

        public void UpdateCategory(OutlookCategory category)
        {
            IEnumerable<OutlookTask> tasksinCategory = taskAndCategoryLoader.Tasks.Where<OutlookTask>(task => task.Category != null && task.Category.Equals(category));
            foreach (OutlookTask task in tasksinCategory)
            {
                UpdateTask(task);
            }
        }
        #endregion

        #region Task CUD

        public OutlookTask CreateTaskInCategory(OutlookCategory category)
        {
            OutlookTask task = new OutlookTask()
            {
                Category = category,
                TaskName = Properties.Resources.Outlook_Task_New,
                IsComplete = false,
                CreationTime = DateTime.Now,
                ActualWork = 0,
                Status = TaskStatus.TaskNotStarted
            };

            Outlook.TaskItem taskitem = outlookApp.CreateItem(Outlook.OlItemType.olTaskItem) as Outlook.TaskItem;
            taskitem = taskAndCategoryLoader.UpdateOutlookTaskItem(taskitem, task);
            taskitem.Save();

            return task;
        }

        public void DeleteTask(OutlookTask task)
        {
            OutlookTask outlookTask = task as OutlookTask;
            Outlook.TaskItem taskitem = outlookApp.Session.GetItemFromID(outlookTask.EntryId) as Outlook.TaskItem;
            taskitem.Delete();
        }

        public void UpdateTask(OutlookTask task)
        {
            OutlookTask outlookTask = task as OutlookTask;
            Outlook.TaskItem taskitem = outlookApp.Session.GetItemFromID(outlookTask.EntryId) as Outlook.TaskItem;
            taskitem = taskAndCategoryLoader.UpdateOutlookTaskItem(taskitem, task as OutlookTask);
            taskitem.Save();
        }
        #endregion

        #region Outlook task update

        private void OutlookItems_ItemChange(object Item)
        {
            if (Item is Outlook.TaskItem)
            {
                Outlook.TaskItem taskItem = Item as Outlook.TaskItem;
                taskAndCategoryLoader.Tasks.ForEach(task =>
                {
                    if (task.EntryId.Equals(taskItem.EntryID))
                    {
                        taskAndCategoryLoader.UpdateTask(task, taskItem, taskAndCategoryLoader.Categories, taskAndCategoryLoader.DefaultCategory);
                        if (TaskChange != null)
                            TaskChange(task);
                    }

                });
            }
        }

        private void OutlookItems_ItemRemove()
        {
            IEnumerable<Outlook.TaskItem> taskEnumerator = outlookItems.Cast<Outlook.TaskItem>();
            List<Outlook.TaskItem> outlookTaskList = new List<Outlook.TaskItem>(taskEnumerator);
            IEnumerable<OutlookTask> toDeleteSubset = taskAndCategoryLoader.Tasks.Where(task => !outlookTaskList.Exists(element => element.EntryID.Equals(task.EntryId)));
            for (int index = 0; index < toDeleteSubset.Count(); index++)
            {
                OutlookTask taskToDelete = toDeleteSubset.ElementAt(index);
                taskAndCategoryLoader.Tasks.Remove(taskToDelete);
                if (TaskRemove != null)
                    TaskRemove(taskToDelete);
            }
        }

        private void OutlookItems_ItemAdd(object Item)
        {
            if (Item is Outlook.TaskItem)
            {
                OutlookTask newTask = new OutlookTask();
                taskAndCategoryLoader.UpdateTask(newTask, Item as Outlook.TaskItem, taskAndCategoryLoader.Categories, taskAndCategoryLoader.DefaultCategory);

                taskAndCategoryLoader.Tasks.Add(newTask);
                if (TaskAdd != null)
                    TaskAdd(newTask);
            }
        }

        #endregion
    }
}
