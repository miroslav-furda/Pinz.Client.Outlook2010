﻿using Com.Pinz.Client.Outlook.Service;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Models
{
    public class CategoryListModel : BindableBase
    {

        private ObservableCollection<OutlookCategory> _categories;
        public ObservableCollection<OutlookCategory> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                SetProperty(ref this._categories, value);
            }
        }

        public DelegateCommand CreateCategory { get; private set; }

        private ICategoryService categoryService;

        [Inject]
        public CategoryListModel(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            CreateCategory = new DelegateCommand(OnCreateCategory);

            Categories = categoryService.ReadAllCategories();
        }

        private void OnCreateCategory()
        {
            categoryService.Create();
        }
    }
}