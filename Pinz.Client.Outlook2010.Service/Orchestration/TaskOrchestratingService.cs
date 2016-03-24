using Com.Pinz.Client.DomainModel.Service;
using Com.Pinz.Client.DomainModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Com.Pinzonline.DomainModel;
using Ninject;
using Com.Pinz.Client.ServiceConsumer.Service;
using Pinz.Client.Outlook2010.Service.OutlookService;
using System.Linq;

namespace Pinz.Client.Outlook2010.Service.Orchestration
{
    internal class TaskOrchestratingService : ITaskWpfService
    {
        private ObservableCollection<Project> projectsObservable;
        private Dictionary<Project, ObservableCollection<Category>> categoriesMap;
        private Dictionary<Category, ObservableCollection<Task>> tasksMap;

        private ITaskRemoteService taskRemoteService;
        private ITaskOutlookService taskOutlookService;

        [Inject]
        public TaskOrchestratingService(ITaskRemoteService taskRemoteService, ITaskOutlookService taskOutlookService)
        {
            this.taskRemoteService = taskRemoteService;
            this.taskOutlookService = taskOutlookService;

            projectsObservable = new ObservableCollection<Project>();
            categoriesMap = new Dictionary<Project, ObservableCollection<Category>>();
            tasksMap = new Dictionary<Category, ObservableCollection<Task>>();

            taskOutlookService.TaskAdd += TaskOutlookService_TaskAdd;
            taskOutlookService.TaskRemove += TaskOutlookService_TaskRemove;
        }


        #region Read

        public ObservableCollection<Project> ReadAllProjectsForCurrentUser()
        {
            projectsObservable.Clear();
            Project outlookProject = taskOutlookService.ReadProjectOutlookDefault();
            projectsObservable.Add(outlookProject);
            taskRemoteService.ReadAllProjectsForCurrentUser().ForEach(projectsObservable.Add);
            return projectsObservable;
        }

        public ObservableCollection<Category> ReadAllCategoriesByProject(Project project)
        {
            ObservableCollection<Category> categories = loadObservableCollection(project, categoriesMap);
            categories.Clear();
            List<Category> catList;
            if (project is OutlookModel.OutlookProject)
            {
                catList = taskOutlookService.ReadAllCategoriesByProject(project);
            }
            else
            {
                catList = taskRemoteService.ReadAllCategoriesByProject(project);
            }
            catList.ForEach(categories.Add);

            return categories;
        }

        public ObservableCollection<Task> ReadAllTasksByCategory(Category category)
        {
            ObservableCollection<Task> tasks = loadObservableCollection(category, tasksMap);
            tasks.Clear();
            List<Task> taskList;
            if (category is OutlookModel.OutlookCategory)
            {
                taskList = taskOutlookService.ReadAllTasksByCategory(category);
            }
            else
            {
                taskList = taskRemoteService.ReadAllTasksByCategory(category);
            }
            taskList.ForEach(tasks.Add);

            return tasks;
        }
        #endregion

        public void MoveTaskToCategory(Task task, Category category)
        {
            Category originalCategory = tasksMap.Keys.Where(c => c.CategoryId == task.CategoryId).Single();
            if (category is OutlookModel.OutlookCategory)
            {
                taskOutlookService.MoveTaskToCategory(task, category);
            }
            else
            {
                taskRemoteService.MoveTaskToCategory(task, category);
            }
            loadObservableCollection(originalCategory, tasksMap).Remove(task);
            loadObservableCollection(category, tasksMap).Add(task);
        }

        public void ChangeTaskStatus(Task task, TaskStatus newStatus)
        {
            if (task is OutlookModel.OutlookTask)
            {
                taskOutlookService.ChangeTaskStatus(task, newStatus);
            }
            else
            {
                taskRemoteService.ChangeTaskStatus(task, newStatus);
            }
        }

        #region Task CUD
        public Task CreateTaskInCategory(Category category)
        {
            Task createdTask;
            if (category is OutlookModel.OutlookCategory)
            {
                createdTask = taskOutlookService.CreateTaskInCategory(category);
            }
            else
            {
                createdTask = taskRemoteService.CreateTaskInCategory(category);
            }
            loadObservableCollection(category, tasksMap).Add(createdTask);
            return createdTask;
        }

        public void UpdateTask(Task task)
        {
            if (task is OutlookModel.OutlookTask)
            {
                taskOutlookService.UpdateTask(task);
            }
            else
            {
                taskRemoteService.UpdateTask(task);
            }
        }

        public void DeleteTask(Task task)
        {
            if (task is OutlookModel.OutlookTask)
            {
                taskOutlookService.DeleteTask(task);
            }
            else
            {
                taskRemoteService.DeleteTask(task);
            }
            Category category = tasksMap.Keys.Where(c => c.CategoryId == task.CategoryId).Single();
            loadObservableCollection(category, tasksMap).Remove(task);
        }
        #endregion

        #region Category CUD
        public Category CreateCategoryInProject(Project project)
        {
            Category createdCategory;
            if (project is OutlookModel.OutlookProject)
            {
                createdCategory = taskOutlookService.CreateCategoryInProject(project);
            }
            else
            {
                createdCategory = taskRemoteService.CreateCategoryInProject(project);
            }
            loadObservableCollection(project, categoriesMap).Add(createdCategory);
            return createdCategory;
        }

        public void UpdateCategory(Category category)
        {
            if (category is OutlookModel.OutlookCategory)
            {
                taskOutlookService.UpdateCategory(category);
            }
            else
            {
                taskRemoteService.UpdateCategory(category);
            }
        }

        public void DeleteCategory(Category category)
        {
            if (category is OutlookModel.OutlookCategory)
            {
                taskOutlookService.DeleteCategory(category);
            }
            else
            {
                taskRemoteService.DeleteCategory(category);
            }
            Project project = categoriesMap.Keys.Where(p => p.ProjectId == category.ProjectId).Single();
            loadObservableCollection(project, categoriesMap).Remove(category);
        }
        #endregion

        #region Listeners
        private void TaskOutlookService_TaskRemove(Task task, Category category)
        {
            ObservableCollection<Task> tasks = loadObservableCollection(category, tasksMap);
            if (tasks.Contains(task))
                tasks.Remove(task);
        }

        private void TaskOutlookService_TaskAdd(Task task, Category category)
        {
            ObservableCollection<Task> tasks = loadObservableCollection(category, tasksMap);
            if (!tasks.Contains(task))
                tasks.Add(task);
        }
        #endregion


        private ObservableCollection<T> loadObservableCollection<K, T>(K key, Dictionary<K, ObservableCollection<T>> dictionary)
          where T : class
          where K : class
        {
            ObservableCollection<T> observableCollection;
            if (dictionary.ContainsKey(key))
            {
                observableCollection = dictionary[key];
            }
            else
            {
                observableCollection = new ObservableCollection<T>();
                dictionary.Add(key, observableCollection);
            }

            return observableCollection;
        }
    }
}
