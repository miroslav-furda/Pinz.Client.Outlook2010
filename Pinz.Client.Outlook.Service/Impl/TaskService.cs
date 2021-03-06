﻿using System.Collections.ObjectModel;
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
        private IOutlookService outlookService;
        private TaskFilter _taskFilter;
        private Dictionary<OutlookCategory, ObservableCollection<OutlookTask>> tasks;

        [Inject]
        public TaskService(IOutlookService outlookService, TaskFilter filter)
        {
            this.outlookService = outlookService;
            this._taskFilter = filter;

            tasks = new Dictionary<OutlookCategory, ObservableCollection<OutlookTask>>();

            outlookService.TaskAdd += TaskDAO_TaskAdd;
            outlookService.TaskChange += TaskDAO_TaskChange;
            outlookService.TaskRemove += TaskDAO_TaskRemove;

            _taskFilter.PropertyChanged += TaskFilter_PropertyChanged;
        }

        public void StartTask(OutlookTask task)
        {
            task.Status = TaskStatus.TaskInProgress;
            task.StartDate = DateTime.Today;
            task.DueDate = DateTime.Today;
            task.DateCompleted = null;
            outlookService.UpdateTask(task);
        }

        public void CompleteTask(OutlookTask task)
        {
            task.Status = TaskStatus.TaskComplete;
            task.DateCompleted = DateTime.Today;
            outlookService.UpdateTask(task);
        }

        public void ReopenTask(OutlookTask task)
        {
            task.Status = TaskStatus.TaskNotStarted;
            task.StartDate = null;
            task.DueDate = null;
            task.DateCompleted = null;
            outlookService.UpdateTask(task);
        }

        public void Update(OutlookTask task)
        {
            outlookService.UpdateTask(task);
        }

        public void Delete(OutlookTask task)
        {
            outlookService.DeleteTask(task);
        }



        public OutlookTask CreateNewTask(OutlookCategory category)
        {
            OutlookTask task = outlookService.CreateTaskInCategory(category);
            return task;
        }

        public void MoveToCategory(OutlookTask sourceItem, OutlookCategory newCategory)
        {
            if( sourceItem.Category != newCategory)
            {
                tasks[sourceItem.Category].Remove(sourceItem);
                sourceItem.Category = newCategory;
                outlookService.UpdateTask(sourceItem);
                tasks[sourceItem.Category].Add(sourceItem);
            }
        }


        public ObservableCollection<OutlookTask> ReadByCategory(OutlookCategory category)
        {
            ObservableCollection<OutlookTask> tasksInCategory = loadTasks(category);
            tasksInCategory.Clear();
            List<OutlookTask> taskList = outlookService.ReadAllTasksByCategory(category);
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

            if (!_taskFilter.Complete)
            {
                retval = taskitem.Status != TaskStatus.TaskComplete;
            }

            if (retval && _taskFilter.DueToday)
            {
                System.DateTime today = System.DateTime.Today;
                retval = taskitem.DueDate.Equals(today);
            }

            if (retval && _taskFilter.InProgress && _taskFilter.NotStarted)
            {
                retval = taskitem.Status == TaskStatus.TaskInProgress ||
                   taskitem.Status == TaskStatus.TaskNotStarted;
            }
            else if (retval && _taskFilter.InProgress)
            {
                retval = taskitem.Status == TaskStatus.TaskInProgress;
            }
            else if (retval && _taskFilter.NotStarted)
            {
                retval = taskitem.Status == TaskStatus.TaskNotStarted;
            }

            return retval;
        }
        #endregion
    }
}
