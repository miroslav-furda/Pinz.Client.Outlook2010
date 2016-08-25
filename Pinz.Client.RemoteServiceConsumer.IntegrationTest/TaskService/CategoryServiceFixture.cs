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

namespace Com.Pinz.Client.RemoteServiceConsumer.TaskService
{
    [TestClass]
    public class CategoryServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IAuthorisationRemoteService authorisationService;
        private ITaskRemoteService taskService;
        private IKernel kernel;
        private Company company;
        private Project project;

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

            pinzService = kernel.Get<IPinzAdminRemoteService>();
            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
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
            project = await service.CreateProjectAsync(project);

            User user = new User()
            {
                EMail = "me@gmail.com",
                FirstName = "Blaha",
                FamilyName = "Boo",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = await service.CreateUserAsync(user);

            await service.AddUserToProjectAsync(user, project, true);
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            var res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateCategory()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category = await taskService.CreateCategoryInProjectAsync(project);

            Assert.IsNotNull(category.CategoryId);
            Assert.IsNotNull(category.ProjectId);
            Assert.IsNotNull(category.Name);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task UpdateCategory()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category =  await taskService.CreateCategoryInProjectAsync(project);
            Assert.IsNotNull(category.CategoryId);


            category.Name = "New name";
            await taskService.UpdateCategoryAsync(category);

            List<Category> categories = await taskService.ReadAllCategoriesByProjectAsync(project);
            Assert.AreEqual(1, categories.Count());
            Assert.AreEqual(category.Name, categories[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task UpdateCategory_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category = await taskService.CreateCategoryInProjectAsync(project);
            Assert.IsNotNull(category.CategoryId);

            category.Name = null;
            await taskService.UpdateCategoryAsync(category);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task DeleteCategory()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category =  await taskService.CreateCategoryInProjectAsync(project);
            Assert.IsNotNull(category.CategoryId);

            await taskService.DeleteCategoryAsync(category);

            List<Category> categories = await taskService.ReadAllCategoriesByProjectAsync(project);
            Assert.AreEqual(0, categories.Count());
        }
    }
}
