using System;
using System.Collections.Generic;
using AutoMapper;
using Ninject;
using System.ServiceModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Properties;
using Com.Pinz.DomainModel;
using Threading = System.Threading.Tasks;
namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class TaskService : ServiceBase, ITaskRemoteService
    {
        private IMapper mapper;

        private ChannelFactory<TaskServiceReference.ITaskService> clientFactory;
        private TaskServiceReference.ITaskService channel;
        private UserNameClientCredentials clientCredentials;

        [Inject]
        public TaskService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<TaskServiceReference.ITaskService> clientFactory, UserNameClientCredentials clientCredentials)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;
            this.clientCredentials = clientCredentials;
        }

        public async Threading.Task<List<Project>> ReadAllProjectsForCurrentUserAsync()
        {
            List<TaskServiceReference.ProjectDO> rProjects = await channel.ReadAllProjectsForUserEmailAsync(clientCredentials.UserName);
            List<Project> projectList = mapper.Map<List<TaskServiceReference.ProjectDO>, List<Project>>(rProjects);
            return projectList;
        }

        public async Threading.Task<List<Category>> ReadAllCategoriesByProjectAsync(Project project)
        {
            List<TaskServiceReference.CategoryDO> rCategories = await channel.ReadAllCategoriesByProjectIdAsync(project.ProjectId);
            List<Category> categoryList = mapper.Map<List<TaskServiceReference.CategoryDO>, List<Category>>(rCategories);
            categoryList.ForEach(c =>
            {
                c.Project = project;
                foreach (Task task in c.Tasks)
                {
                    task.Category = c;
                }
            });
            return categoryList;
        }

        public async Threading.Task<List<Task>> ReadAllTasksByCategoryAsync(Category category)
        {
            List<TaskServiceReference.TaskDO> rTasks = await channel.ReadAllTasksByCategoryIdAsync(category.CategoryId);
            List<Task> taskList = mapper.Map<List<TaskServiceReference.TaskDO>, List<Task>>(rTasks);
            taskList.ForEach(i => i.Category = category);
            return taskList;
        }

        public async Threading.Task MoveTaskToCategoryAsync(Task task, Category category)
        {
            task.CategoryId = category.CategoryId;
            task.Category = category;
            await UpdateTaskAsync(task);
        }

        public async Threading.Task ChangeTaskStatusAsync(Task task, TaskStatus newStatus)
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
            await UpdateTaskAsync(task);
        }

       

        #region Category CUD
        public async Threading.Task<Category> CreateCategoryInProjectAsync(Project project)
        {
            Category category = new Category()
            {
                ProjectId = project.ProjectId,
                Name = Resources.TaskService_NewCategoryName
            };
            TaskServiceReference.CategoryDO rCategory = await channel.CreateCategoryAsync(mapper.Map<TaskServiceReference.CategoryDO>(category));
            mapper.Map(rCategory, category);
            category.Project = project;
            return category;
        }

        public async Threading.Task DeleteCategoryAsync(Category category)
        {
            await channel.DeleteCategoryAsync(mapper.Map<TaskServiceReference.CategoryDO>(category));
        }

        public async Threading.Task UpdateCategoryAsync(Category category)
        {
            TaskServiceReference.CategoryDO rCategory = await channel.UpdateCategoryAsync(mapper.Map<TaskServiceReference.CategoryDO>(category));
            mapper.Map(rCategory, category);
        }
        #endregion

        #region Task CUD
        public async Threading.Task<Task> CreateTaskInCategoryAsync(Category category)
        {
            Task task = new Task()
            {
                CategoryId = category.CategoryId,
                TaskName = Resources.TaskService_NewTaskName,
                IsComplete = false,
                CreationTime = DateTime.Now,
                ActualWork = 0,
                Status = TaskStatus.TaskNotStarted
            };
            TaskServiceReference.TaskDO rTask = await channel.CreateTaskAsync(mapper.Map<TaskServiceReference.TaskDO>(task), clientCredentials.UserName);
            mapper.Map(rTask, task);
            task.Category = category;
            return task;
        }

        public async Threading.Task DeleteTaskAsync(Task task)
        {
            await channel.DeleteTaskAsync(mapper.Map<TaskServiceReference.TaskDO>(task));
        }

        public async Threading.Task UpdateTaskAsync(Task task)
        {
            TaskServiceReference.TaskDO rTask = await channel.UpdateTaskAsync(mapper.Map<TaskServiceReference.TaskDO>(task));
            mapper.Map(rTask, task);
        }


        #endregion

        public override void OpenChannel()
        {
            channel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(channel as ICommunicationObject);
        }
    }
}
