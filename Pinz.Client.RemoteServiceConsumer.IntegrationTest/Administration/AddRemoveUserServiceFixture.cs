using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Pinz.Client.RemoteServiceConsumer.IntegrationTest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Pinz.Client.RemoteServiceConsumer.Administration
{
    [TestClass]
    public class AddRemoveUserServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private ITaskRemoteService taskService;
        private IKernel kernel;
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

            Company company1 = new Company()
            {
                Name = "Pinz Online"
            };
            company = await pinzService.CreateCompanyAsync(company1);

            project = new Project()
            {
                CompanyId = company.CompanyId,
                Name = "My test project",
                Description = "Description"
            };
            await service.CreateProjectAsync(project);

            user = new User()
            {
                EMail = "me@gmail.com",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = await service.CreateUserAsync(user);
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            credentials.UserName = TestUserCredentials.UserName;
            credentials.Password = TestUserCredentials.Password;
            credentials.UpdateCredentialsForAllFactories();

            System.Threading.Tasks.Task res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task AddUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            await service.AddUserToProjectAsync(user, project, true);

            credentials.UserName = user.EMail;
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();
            List<Project> projects = await taskService.ReadAllProjectsForCurrentUserAsync();
            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual(project.ProjectId, projects[0].ProjectId);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task RemoveUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            await service.AddUserToProjectAsync(user, project, true);

            credentials.UserName = user.EMail;
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            List<Project> projects = await taskService.ReadAllProjectsForCurrentUserAsync();
            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual(project.ProjectId, projects[0].ProjectId);

            await service.RemoveUserFromProjectAsync(user, project);

            List<Project> projects2 = await taskService.ReadAllProjectsForCurrentUserAsync();
            Assert.AreEqual(0, projects2.Count());

        }
    }
}
