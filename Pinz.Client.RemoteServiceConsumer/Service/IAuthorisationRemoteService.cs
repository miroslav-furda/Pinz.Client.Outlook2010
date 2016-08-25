using Com.Pinz.Client.DomainModel;
using System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface IAuthorisationRemoteService
    {
        Task<bool> IsUserProjectAdminAsync(User user, Project project);

        Task<bool> IsUserComapnyAdminAsync(User user);

        Task<User> ReadUserByEmailAsync(string email);
    }
}
