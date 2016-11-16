using System;
using System.Collections.Generic;
using Ninject;
using AutoMapper;
using System.ServiceModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.DomainModel;
using Threading = System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class AdministrationService : ServiceBase, IAdministrationRemoteService
    {
        private IMapper mapper;
        private ChannelFactory<AdministrationServiceReference.IAdministrationService> clientFactory;
        private AdministrationServiceReference.IAdministrationService adminChannel;

        [Inject]
        public AdministrationService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<AdministrationServiceReference.IAdministrationService> clientFactory)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;
        }

        public async Threading.Task<bool> CanCreateNewProject(Guid companyId)
        {
            return await adminChannel.CanCreateProjectAsync(companyId);
        }

        public async Threading.Task<User> InviteNewUserAsync(string newUserEmail, Project project, User invitingUser)
        {
            AdministrationServiceReference.UserDO user = await adminChannel.InviteNewUserAsync(newUserEmail, project.ProjectId, invitingUser.UserId);
            return mapper.Map<User>(user);
        }

        public async Threading.Task SetProjectAdminFlagAsync(Guid userId, Guid projectId, bool isProjectAdmin)
        {
            await adminChannel.SetProjectAdminFlagAsync(userId, projectId, isProjectAdmin);
        }

        public async Threading.Task<bool> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword, string newPassword2)
        {
            return await adminChannel.ChangeUserPasswordAsync(user.UserId, oldPassword, newPassword, newPassword2);
        }

        public async Threading.Task AddUserToProjectAsync(User user, Project project, bool isProjectAdmin)
        {
            await adminChannel.AddUserToProjectAsync(user.UserId, project.ProjectId, isProjectAdmin);
        }

        public async Threading.Task RemoveUserFromProjectAsync(User user, Project project)
        {
            await adminChannel.RemoveUserFromProjectAsync(user.UserId, project.ProjectId);
        }

        public async Threading.Task<List<User>> ReadAllUsersForCompanyAsync(Guid companyId)
        {
            List<AdministrationServiceReference.UserDO> rUsers = await adminChannel.ReadAllUsersForCompanyIdAsync(companyId);
            List<User> users = mapper.Map<List<AdministrationServiceReference.UserDO>, List<User>>(rUsers);
            return users;
        }

        public async Threading.Task<List<User>> ReadAllUsersByProjectAsync(Project project)
        {
            List<AdministrationServiceReference.UserDO> rUsers = await adminChannel.ReadAllUsersByProjectAsync(project.ProjectId);
            List<User> userList = mapper.Map<List<AdministrationServiceReference.UserDO>, List<User>>(rUsers);
            return userList;
        }

        public async Threading.Task<List<ProjectUser>> ReadAllProjectUsersInProjectAsync(Project project)
        {
            List<AdministrationServiceReference.ProjectUserDO> rProjectUsers = await adminChannel.ReadAllProjectUsersInProjectAsync(project.ProjectId);
            List<ProjectUser> userList = mapper.Map<List<AdministrationServiceReference.ProjectUserDO>, List<ProjectUser>>(rProjectUsers);
            return userList;

        }


        public async Threading.Task<Company> ReadCompanyByIdAsync(Guid id)
        {
            AdministrationServiceReference.CompanyDO rCompany = await adminChannel.ReadCompanyByIdAsync(id);
            return mapper.Map<Company>(rCompany);
        }

        public async Threading.Task<List<Project>> ReadProjectsForCompanyAsync(Company company)
        {
            List<AdministrationServiceReference.ProjectDO> rProjects = await adminChannel.ReadProjectsForCompanyIdAsync(company.CompanyId);
            List<Project> projectList = mapper.Map<List<AdministrationServiceReference.ProjectDO>, List<Project>>(rProjects);
            return projectList;
        }

        public async Threading.Task<List<Project>> ReadAdminProjectsForUserAsync(User user)
        {
            List<AdministrationServiceReference.ProjectDO> rProjects = await adminChannel.ReadAdminProjectsForUserAsync(user.UserId);
            List<Project> projectList = mapper.Map<List<AdministrationServiceReference.ProjectDO>, List<Project>>(rProjects);
            return projectList;
        }


        #region Project CUD
        public async Threading.Task<Project> CreateProjectAsync(Project project)
        {
            AdministrationServiceReference.ProjectDO rProject = await adminChannel.CreateProjectAsync(mapper.Map<AdministrationServiceReference.ProjectDO>(project));
            mapper.Map(rProject, project);
            return project;
        }

        public async Threading.Task UpdateProjectAsync(Project project)
        {
            AdministrationServiceReference.ProjectDO rProject = await adminChannel.UpdateProjectAsync(mapper.Map<AdministrationServiceReference.ProjectDO>(project));
            mapper.Map(rProject, project);
        }

        public async Threading.Task DeleteProjectAsync(Project project)
        {
            await adminChannel.DeleteProjectAsync(mapper.Map<AdministrationServiceReference.ProjectDO>(project));
        }
        #endregion

        #region User CUD
        public async Threading.Task DeleteUserAsync(User user)
        {
            await adminChannel.DeleteUserAsync(mapper.Map<AdministrationServiceReference.UserDO>(user));
        }

        public async Threading.Task<User> CreateUserAsync(User user)
        {
            AdministrationServiceReference.UserDO rUser = mapper.Map<AdministrationServiceReference.UserDO>(user);
            rUser = await adminChannel.CreateUserAsync(rUser);
            mapper.Map(rUser, user);
            return user;
        }

        public async Threading.Task UpdateUserAsync(User user)
        {
            AdministrationServiceReference.UserDO rUser = await adminChannel.UpdateUserAsync(mapper.Map<AdministrationServiceReference.UserDO>(user));
            mapper.Map(rUser, user);
        }
        #endregion 

        public override void OpenChannel()
        {
            adminChannel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(adminChannel as ICommunicationObject);
        }
    }
}
