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
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.TaskService
{
    [TestClass]
    public class TaskServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private ITaskRemoteService taskService;
        private IKernel kernel;
        private Category category;
        private Company company;
        private IAuthorisationRemoteService authorisationService;

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

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = TestUserCredentials.UserName;
            credentials.Password = TestUserCredentials.Password;
            credentials.UpdateCredentialsForAllFactories();

            company = new Company()
            {
                Name = "Pinz Online"
            };
            company = await pinzService.CreateCompanyAsync(company);

            Project project = new Project()
            {
                CompanyId = company.CompanyId,
                Name = "My test project",
                Description = "Descirption"
            };
            await service.CreateProjectAsync(project);

            User user = new User()
            {
                EMail = "me@gmail.com",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = await service.CreateUserAsync(user);

            await service.AddUserToProjectAsync(user, project, true);

            category = await taskService.CreateCategoryInProjectAsync(project);
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            var res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateTask()
        {
            Assert.AreNotEqual(Guid.Empty, category.CategoryId);

            Task task = await taskService.CreateTaskInCategoryAsync(category);

            Assert.IsNotNull(task.TaskId);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task UpdateTask()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            Task task = await taskService.CreateTaskInCategoryAsync(category);
            Assert.IsNotNull(task.TaskId);

            task.TaskName = "New name";
            await taskService.UpdateTaskAsync(task);

            List<Task> tasks = await taskService.ReadAllTasksByCategoryAsync(category);
            Assert.AreEqual(1, tasks.Count());
            Assert.AreEqual(task.TaskName, tasks[0].TaskName);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task UpdateTask_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            Task task = await taskService.CreateTaskInCategoryAsync(category);
            Assert.IsNotNull(task.TaskId);

            task.TaskName = null;
            await taskService.UpdateTaskAsync(task);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task DeleteTask()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            Task task = await taskService.CreateTaskInCategoryAsync(category);
            Assert.IsNotNull(task.TaskId);

            await taskService.DeleteTaskAsync(task);

            List<Task> tasks = await taskService.ReadAllTasksByCategoryAsync(category);
            Assert.AreEqual(0, tasks.Count());
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
