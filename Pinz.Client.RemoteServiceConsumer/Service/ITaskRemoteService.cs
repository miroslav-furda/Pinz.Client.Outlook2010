using Com.Pinz.Client.DomainModel;
using Com.Pinz.DomainModel;
using System.Collections.Generic;
using Threading = System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface ITaskRemoteService
    {
        Threading.Task<List<Task>> ReadAllTasksByCategoryAsync(Category category);

        Threading.Task<List<Category>> ReadAllCategoriesByProjectAsync(Project project);

        Threading.Task<List<Project>> ReadAllProjectsForCurrentUserAsync();

        Threading.Task MoveTaskToCategoryAsync(Task task, Category category);

        Threading.Task ChangeTaskStatusAsync(Task task, TaskStatus newStatus);

        Threading.Task<Task> CreateTaskInCategoryAsync(Category category);

        Threading.Task UpdateTaskAsync(Task task);

        Threading.Task DeleteTaskAsync(Task task);

        Threading.Task<Category> CreateCategoryInProjectAsync(Project project);

        Threading.Task UpdateCategoryAsync(Category category);

        Threading.Task DeleteCategoryAsync(Category category);
    }
}
