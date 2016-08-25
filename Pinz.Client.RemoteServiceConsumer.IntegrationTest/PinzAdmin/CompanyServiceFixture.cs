using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Pinz.Client.RemoteServiceConsumer.IntegrationTest;
using System;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.PinzAdmin
{

    [TestClass]
    public class CompanyServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService adminService;
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

            pinzService = kernel.Get<IPinzAdminRemoteService>();
            adminService = kernel.Get<IAdministrationRemoteService>();

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = TestUserCredentials.UserName;
            credentials.Password = TestUserCredentials.Password;
            credentials.UpdateCredentialsForAllFactories();

            Company company1 = new Company()
            {
                Name = "Pinz Online"
            };
            company =  await pinzService.CreateCompanyAsync(company1);
        }

        [TestCleanup()]
        public void UnloadKernel()
        {
            var res = pinzService.DeleteCompanyAsync(company);
            res.Wait();

            kernel.Dispose();
        }

        [TestMethod]
        public void CreateCompany()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task CreateCompany_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Company company2 = new Company();

            await pinzService.CreateCompanyAsync(company2);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task UpdateCompany()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            company.Name = "New name";
            await pinzService.UpdateCompanyAsync(company);

            Company company2 = await adminService.ReadCompanyByIdAsync(company.CompanyId);
            Assert.AreEqual(company.Name, company2.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), "Validation failed.")]
        public async System.Threading.Tasks.Task UpdateCompany_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            company.Name = null;

            await pinzService.UpdateCompanyAsync(company);
        }
    }
}
