using Com.Pinz.Client.DomainModel;
using Threading = System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface IPinzAdminRemoteService
    {
        Threading.Task<Company> CreateCompanyAsync(Company company);

        Threading.Task UpdateCompanyAsync(Company company);

        Threading.Task DeleteCompanyAsync(Company company);
    }
}
