using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Pinz.Client.RemoteServiceConsumer.IntegrationTest;
using System;

namespace Com.Pinz.Client.RemoteServiceConsumer.Authorisation
{
    [TestClass]
    public class AuthorisationServiceTestFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IAuthorisationRemoteService authorisationService;
        private IKernel kernel;
        private Company company;
        private Project project;
        private User user;


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
            authorisationService = kernel.Get<IAuthorisationRemoteService>();

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
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
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            var res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IsUserCompanyAdmin()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            bool isCompanyAdmin = await authorisationService.IsUserComapnyAdminAsync(user);

            Assert.IsTrue(isCompanyAdmin);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ReadUserByEmailAsync()
        {
            User rUser = await authorisationService.ReadUserByEmailAsync(user.EMail);

            Assert.AreEqual(user.UserId, rUser.UserId);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IsUserProjectAdmin_False()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            bool isCompanyAdmin = await authorisationService.IsUserProjectAdminAsync(user, project);

            Assert.IsFalse(isCompanyAdmin);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IsUserProjectAdmin_True()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            await service.AddUserToProjectAsync(user, project, true);

            bool isCompanyAdmin = await authorisationService.IsUserProjectAdminAsync(user, project);

            Assert.IsTrue(isCompanyAdmin);
        }
    }
}
