using AutoMapper;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using System.ServiceModel;
using Threading = System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class PinzAdminService : ServiceBase, IPinzAdminRemoteService
    {
        private IMapper mapper;

        private ChannelFactory<PinzAdminServiceReference.IPinzAdminService> clientFactory;
        private PinzAdminServiceReference.IPinzAdminService channel;

        [Inject]
        public PinzAdminService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<PinzAdminServiceReference.IPinzAdminService> clientFactory)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;
        }

        public override void OpenChannel()
        {
            channel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(channel as ICommunicationObject);
        }

        public async Threading.Task<Company> CreateCompanyAsync(Company company)
        {
            PinzAdminServiceReference.CompanyDO rCompIn = mapper.Map<PinzAdminServiceReference.CompanyDO>(company);
            PinzAdminServiceReference.CompanyDO rCompany = await channel.CreateCompanyAsync(rCompIn);
            mapper.Map(rCompany, company);
            return company;
        }

        public async Threading.Task UpdateCompanyAsync(Company company)
        {
            await channel.UpdateCompanyAsync(mapper.Map<PinzAdminServiceReference.CompanyDO>(company));
        }

        public async Threading.Task DeleteCompanyAsync(Company company)
        {
            await channel.DeleteCompanyAsync(mapper.Map<PinzAdminServiceReference.CompanyDO>(company));
        }
    }
}
