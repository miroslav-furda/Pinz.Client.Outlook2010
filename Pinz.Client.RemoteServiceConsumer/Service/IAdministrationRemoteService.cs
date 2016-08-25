using Com.Pinz.Client.DomainModel;
using System;
using System.Collections.Generic;
using Threading = System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface IAdministrationRemoteService
    {
        Threading.Task<User> InviteNewUserAsync(string newUserEmail, Project project, User invitingUser);

        Threading.Task SetProjectAdminFlagAsync(Guid userId, Guid projectId, bool isProjectAdmin);

        Threading.Task<List<User>> ReadAllUsersByProjectAsync(Project project);

        Threading.Task<List<ProjectUser>> ReadAllProjectUsersInProjectAsync(Project project);

        Threading.Task<List<Project>> ReadAdminProjectsForUserAsync(User user);

        Threading.Task<bool> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword, string newPassword2);

        Threading.Task<List<Project>> ReadProjectsForCompanyAsync(Company company);

        Threading.Task<List<User>> ReadAllUsersForCompanyAsync(Guid companyId);

        Threading.Task<Company> ReadCompanyByIdAsync(Guid id);

        Threading.Task AddUserToProjectAsync(User user, Project project, bool isProjectAdmin);

        Threading.Task RemoveUserFromProjectAsync(User user, Project project);

        Threading.Task<Project> CreateProjectAsync(Project project);

        Threading.Task UpdateProjectAsync(Project project);

        Threading.Task DeleteProjectAsync(Project project);

        Threading.Task<User> CreateUserAsync(User user);

        Threading.Task UpdateUserAsync(User user);

        Threading.Task DeleteUserAsync(User user);
    }
}
