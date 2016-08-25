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
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.Administration
{
    [TestClass]
    public class ProjectServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IKernel kernel;
        private Company company;
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

            credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = TestUserCredentials.UserName;
            credentials.Password = TestUserCredentials.Password;
            credentials.UpdateCredentialsForAllFactories();

            Company company1 = new Company()
            {
                Name = "Pinz Online"
            };
            company = await pinzService.CreateCompanyAsync(company1);
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            var res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task  CreateProject()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Descirption";

            await service.CreateProjectAsync(project);

            Assert.IsNotNull(project.ProjectId);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ReadAllProjects()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Descirption";
            await service.CreateProjectAsync(project);
            Assert.IsNotNull(project.ProjectId);

            List<Project> projects = await service.ReadProjectsForCompanyAsync(company);
            Assert.AreEqual(1, projects.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task  CreateProject_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Description = "Descirption";

            await service.CreateProjectAsync(project);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task  UpdateProject()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Description";

            await service.CreateProjectAsync(project);
            Assert.IsNotNull(project.ProjectId);

            project.Name = "New name";
            await service.UpdateProjectAsync(project);

            List<Project> projects = await service.ReadProjectsForCompanyAsync(company);
            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual(project.Name, projects[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task  UpdateProject_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Description";

            await service.CreateProjectAsync(project);
            Assert.IsNotNull(project.ProjectId);

            project.Name = null;
            await service.UpdateProjectAsync(project);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task  DeleteProject()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Description";

            await service.CreateProjectAsync(project);
            Assert.IsNotNull(project.ProjectId);

            await service.DeleteProjectAsync(project);

            List<Project> projects = await service.ReadProjectsForCompanyAsync(company);
            Assert.AreEqual(0, projects.Count());
        }
    }
}
