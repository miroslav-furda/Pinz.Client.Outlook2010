using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Pinz.Client.RemoteServiceConsumer.IntegrationTest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Pinz.Client.RemoteServiceConsumer.TaskService
{
    [TestClass]
    public class ReadTaskServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private ITaskRemoteService taskService;
        private IKernel kernel;
        private Category category;
        private Company company;
        private Project project;
        private User user;
        private IAuthorisationRemoteService authorisationService;
        private UserNameClientCredentials credentials;

        [TestInitialize]
        public void InitTest()
        {
            System.Threading.Tasks.Task res = InitializeKernelAsync();
            res.Wait();
        }

        public async System.Threading.Tasks.Task InitializeKernelAsync()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());
            Mock<IServiceRunningIndicator> servMock = new Mock<IServiceRunningIndicator>();
            kernel.Bind<IServiceRunningIndicator>().ToConstant(servMock.Object);

            service = kernel.Get<IAdministrationRemoteService>();
            pinzService = kernel.Get<IPinzAdminRemoteService>();
            taskService = kernel.Get<ITaskRemoteService>();
            authorisationService = kernel.Get<IAuthorisationRemoteService>();

            credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = TestUserCredentials.UserName;
            credentials.Password = TestUserCredentials.Password;
            credentials.UpdateCredentialsForAllFactories();


            company = new Company()
            {
                Name = "Pinz Online"
            };
            company = await pinzService.CreateCompanyAsync(company);

            project = new Project()
            {
                CompanyId = company.CompanyId,
                Name = "My test project",
                Description = "Descirption"
            };
            await service.CreateProjectAsync(project);

            user = new User()
            {
                EMail = "me@gmail.com",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = await service.CreateUserAsync(user);

            category = await taskService.CreateCategoryInProjectAsync(project);
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            credentials.UserName = TestUserCredentials.UserName;
            credentials.Password = TestUserCredentials.Password;
            credentials.UpdateCredentialsForAllFactories();

            var res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ReadAllTasksByCategory()
        {
            Assert.AreNotEqual(Guid.Empty, category.CategoryId);

            Task task = await taskService.CreateTaskInCategoryAsync(category);
            Assert.IsNotNull(task.TaskId);

            List<Task> tasks = await taskService.ReadAllTasksByCategoryAsync(category);

            Assert.AreEqual(1, tasks.Count());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ReadAllCategoriesByProject()
        {
            List<Category> categories = await taskService.ReadAllCategoriesByProjectAsync(project);

            Assert.AreEqual(1, categories.Count());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ReadAllProjectsForUser()
        {
            Assert.AreNotEqual(Guid.Empty, category.CategoryId);
            Task task = await taskService.CreateTaskInCategoryAsync(category);
            Assert.IsNotNull(task.TaskId);

            await service.AddUserToProjectAsync(user, project, true);

            credentials.UserName = user.EMail;
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            List<Project> projects = await taskService.ReadAllProjectsForCurrentUserAsync();

            Assert.AreEqual(1, projects.Count());
        }

        private Task createTask()
        {
            Task task = new Task()
            {
                TaskName = "TaskName",
                CreationTime = DateTime.Now,
                IsComplete = false,
                ActualWork = 0,
                Status = TaskStatus.TaskNotStarted,
                CategoryId = category.CategoryId
            };
            return task;
        }
    }
}
