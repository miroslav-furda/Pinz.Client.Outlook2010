using System.Collections.ObjectModel;
using System.Collections.Generic;
using Ninject;
using System;
using Com.Pinz.Client.Outlook.Service.DAO;
using Com.Pinz.Client.Outlook.Service.Model;
using Com.Pinz.Client.Outlook.Service.Properties;
using Com.Pinz.Client.Commons.Model;

namespace Com.Pinz.Client.Outlook.Service.Impl
{
    public class TaskService : ITaskService
    {
        private ITaskDAO taskDAO;
        private TaskFilter taskFilter;
        private Dictionary<OutlookCategory, ObservableCollection<OutlookTask>> tasks;

        [Inject]
        public TaskService(ITaskDAO taskDAO, TaskFilter filter)
        {
            this.taskDAO = taskDAO;
            this.taskFilter = filter;

            tasks = new Dictionary<OutlookCategory, ObservableCollection<OutlookTask>>();

            taskDAO.TaskAdd += TaskDAO_TaskAdd;
            taskDAO.TaskChange += TaskDAO_TaskChange;
            taskDAO.TaskRemove += TaskDAO_TaskRemove;

            taskFilter.PropertyChanged += TaskFilter_PropertyChanged;
        }

        public void StartTask(OutlookTask task)
        {
            task.Status = TaskStatus.TaskInProgress;
            task.StartDate = DateTime.Today;
            task.DueDate = DateTime.Today;
            task.DateCompleted = null;
            taskDAO.update(task);
        }

        public void CompleteTask(OutlookTask task)
        {
            task.Status = TaskStatus.TaskComplete;
            task.DateCompleted = DateTime.Today;
            taskDAO.update(task);
        }

        public void ReopenTask(OutlookTask task)
        {
            task.Status = TaskStatus.TaskNotStarted;
            task.StartDate = null;
            task.DueDate = null;
            task.DateCompleted = null;
            taskDAO.update(task);
        }

        public void Update(OutlookTask task)
        {
            taskDAO.update(task);
        }

        public void Delete(OutlookTask task)
        {
            taskDAO.delete(task);
        }



        public OutlookTask CreateNewTask(OutlookCategory category)
        {
            OutlookTask task = new OutlookTask();
            task.TaskName = Resources.New_Task;
            task.CreationTime = DateTime.Today;
            task.Category = category;
            task.Status = TaskStatus.TaskNotStarted;

            taskDAO.create(task);
            return task;
        }

        public void MoveToCategory(OutlookTask sourceItem, OutlookCategory newCategory)
        {
            if( sourceItem.Category != newCategory)
            {
                tasks[sourceItem.Category].Remove(sourceItem);
                sourceItem.Category = newCategory;
                taskDAO.update(sourceItem);
                tasks[sourceItem.Category].Add(sourceItem);
            }
        }


        public ObservableCollection<OutlookTask> ReadByCategory(OutlookCategory category)
        {
            ObservableCollection<OutlookTask> tasksInCategory = loadTasks(category);
            tasksInCategory.Clear();
            List<OutlookTask> taskList = taskDAO.readAll();
            taskList.ForEach(item =>
            {
                if (filter(item) && (category == null || category.Equals(item.Category)))
                    tasksInCategory.Add(item);
            });

            return tasksInCategory;
        }

        private ObservableCollection<OutlookTask> loadTasks(OutlookCategory category)
        {
            ObservableCollection<OutlookTask> tasksInCategory;
            if (tasks.ContainsKey(category))
            {
                tasksInCategory = tasks[category];
            }
            else
            {
                tasksInCategory = new ObservableCollection<OutlookTask>();
                tasks.Add(category, tasksInCategory);
            }

            return tasksInCategory;
        }


        #region Event handlers
        private void TaskDAO_TaskRemove(OutlookTask task)
        {
            foreach (ObservableCollection<OutlookTask> tasksInCategory in tasks.Values)
                tasksInCategory.Remove(task);
        }

        private void TaskDAO_TaskChange(OutlookTask task)
        {
            // do I nedd to do anything?
        }

        private void TaskDAO_TaskAdd(OutlookTask task)
        {
            ObservableCollection<OutlookTask> tasksInCategory = loadTasks(task.Category);
            tasksInCategory.Add(task);
        }
        #endregion

        #region private Filter

        private void TaskFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (OutlookCategory category in tasks.Keys)
            {
                ReadByCategory(category);
            }
        }

        private bool filter(OutlookTask taskitem)
        {
            bool retval = true;

            if (!taskFilter.Complete)
            {
                retval = taskitem.IsComplete.Equals(false);
            }

            if (retval && taskFilter.DueToday)
            {
                System.DateTime today = System.DateTime.Today;
                retval = taskitem.DueDate.Equals(today);
            }

            if (retval && taskFilter.InProgress)
            {
                retval = taskitem.Status.Equals(TaskStatus.TaskInProgress);
                if (taskFilter.NotStarted)
                {
                    retval = taskitem.Status.Equals(TaskStatus.TaskNotStarted);
                }
            }
            return retval;
        }
        #endregion
    }
}
