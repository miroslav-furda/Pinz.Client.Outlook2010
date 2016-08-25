using AutoMapper;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using System.ServiceModel;
using System;
using System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class AuthorisationService : ServiceBase, IAuthorisationRemoteService
    {
        private IMapper mapper;

        private ChannelFactory<AuthorisationServiceReference.IAuthorisationService> clientFactory;
        private AuthorisationServiceReference.IAuthorisationService channel;

        [Inject]
        public AuthorisationService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<AuthorisationServiceReference.IAuthorisationService> clientFactory)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;

        }
        
        public async Task<bool> IsUserComapnyAdminAsync(User user)
        {
            return await channel.IsUserComapnyAdminAsync(user.UserId);
        }

        public async Task<bool> IsUserProjectAdminAsync(User user, Project project)
        {
            return await channel.IsUserProjectAdminAsync(user.UserId, project.ProjectId);
        }

        public async Task<User> ReadUserByEmailAsync(string email)
        {
            AuthorisationServiceReference.UserDO user = await channel.ReadUserByEmailAsync(email);
            return mapper.Map<User>(user);
        }

        public override void OpenChannel()
        {
            channel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(channel as ICommunicationObject);
        }
    }
}
