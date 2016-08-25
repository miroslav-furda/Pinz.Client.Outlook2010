using System.Collections.ObjectModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;
using Prism.Events;
using System;
using Com.Pinz.Client.Commons.Event;
using Com.Pinz.Client.DomainModel;
using System.Collections.Generic;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryListModel : BindableBase
    {
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set
            {
                if (SetProperty(ref _project, value))
                    LoadCategories();
            }
        }

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        public AwaitableDelegateCommand CreateCategory { get; private set; }

        private readonly ITaskRemoteService _taskService;
        private readonly IAdministrationRemoteService _adminService;
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        public CategoryListModel(ITaskRemoteService taskService, IAdministrationRemoteService adminService, IEventAggregator eventAggregator)
        {
            this._taskService = taskService;
            this._adminService = adminService;
            this._eventAggregator = eventAggregator;
            CreateCategory = new AwaitableDelegateCommand(OnCreateCategory);
            Categories = new ObservableCollection<Category>();
        }

        private async System.Threading.Tasks.Task OnCreateCategory()
        {
            try
            {
                Category newCategory = await _taskService.CreateCategoryInProjectAsync(Project);
                Categories.Add(newCategory);
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
        }

        private async System.Threading.Tasks.Task LoadCategories()
        {
            Categories.Clear();
            if (Project != null)
            {
                try
                {
                    Project.Categories = Categories;
                    List<Category> categories = await _taskService.ReadAllCategoriesByProjectAsync(Project);
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }

                    List<User> users = await _adminService.ReadAllUsersByProjectAsync(Project);
                    Project.ProjectUsers.Clear();
                    foreach (var user in users)
                    {
                        Project.ProjectUsers.Add(user);
                    }
                }
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
        }
    }
}