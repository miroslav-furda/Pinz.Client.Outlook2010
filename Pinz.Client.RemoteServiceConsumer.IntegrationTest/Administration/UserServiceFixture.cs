using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Pinz.Client.RemoteServiceConsumer.IntegrationTest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.Administration
{
    [TestClass]
    public class UserServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IKernel kernel;
        private Company company;

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

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
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
        public async System.Threading.Tasks.Task CreateUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            User user = new User();
            user.EMail = "me@hotmail.sk";
            user.IsCompanyAdmin = true;
            user.CompanyId = company.CompanyId;
            user.FirstName = "Miro";
            user.FamilyName = "Furda";

            await service.CreateUserAsync(user);

            Assert.IsNotNull(user.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task CreateUser_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            User user = new User();
            user.EMail = null;
            user.IsCompanyAdmin = true;
            user.CompanyId = company.CompanyId;
            user.FirstName = "Miro";
            user.FamilyName = "Furda";

            await service.CreateUserAsync(user);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task UpdateUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            User user = new User();
            user.EMail = "me@hotmail.com";
            user.IsCompanyAdmin = true;
            user.CompanyId = company.CompanyId;
            user.FirstName = "Miro";
            user.FamilyName = "Furda";

            await service.CreateUserAsync(user);
            Assert.IsNotNull(user.UserId);

            user.FamilyName = "Neungamat";
            await service.UpdateUserAsync(user);

            List<User> users = await service.ReadAllUsersForCompanyAsync(company.CompanyId);
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual(user.FamilyName, users[0].FamilyName);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task UpdateUser_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            User user = new User();
            user.EMail = "me@hotmail.com";
            user.IsCompanyAdmin = true;
            user.CompanyId = company.CompanyId;
            user.FirstName = "Miro";
            user.FamilyName = "Furda";

            await service.CreateUserAsync(user);
            Assert.IsNotNull(user.UserId);

            user.EMail = null;
            await service.UpdateUserAsync(user);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task DeleteUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            User user = new User();
            user.EMail = "me@hotmail.com";
            user.IsCompanyAdmin = true;
            user.CompanyId = company.CompanyId;
            user.FirstName = "Miro";
            user.FamilyName = "Furda";

            await service.CreateUserAsync(user);
            Assert.IsNotNull(user.UserId);

            await service.DeleteUserAsync(user);
            List<User> users = await service.ReadAllUsersForCompanyAsync(company.CompanyId);
            Assert.AreEqual(0, users.Count());
        }

    }
}
