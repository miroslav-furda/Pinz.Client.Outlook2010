using Com.Pinz.Client.DomainModel.Model;
using Com.Pinzonline.DomainModel;
using System.Collections.Generic;

namespace Pinz.Client.Outlook2010.Service.OutlookService
{
    public interface ITaskOutlookService : TaskEvents_Event
    {
        List<Task> ReadAllTasksByCategory(Category category);

        List<Category> ReadAllCategoriesByProject(Project project);

        Project ReadProjectOutlookDefault();

        void MoveTaskToCategory(Task task, Category category);

        void ChangeTaskStatus(Task task, TaskStatus newStatus);

        Task CreateTaskInCategory(Category category);

        void UpdateTask(Task task);

        void DeleteTask(Task task);

        Category CreateCategoryInProject(Project project);

        void UpdateCategory(Category category);

        void DeleteCategory(Category category);
    }
}
