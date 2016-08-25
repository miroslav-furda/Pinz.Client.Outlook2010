﻿using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.DomainModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.DomainModel
{
    public class Category : BindableValidationBase, ICategory
    {
        public Guid CategoryId
        {
            get; set;
        }

        private string _name;
        [Required]//(ErrorMessageResourceName = "Category_Name_Required", ErrorMessageResourceType = typeof(Resources))]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }

        private Guid _projectId;
        [Required]//(ErrorMessageResourceName = "Category_Project_Required", ErrorMessageResourceType = typeof(Resources))]
        public Guid ProjectId
        {
            get { return _projectId; }
            set { SetProperty(ref this._projectId, value); }
        }

        private Project _project;
        public Project Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }

        public ObservableCollection<Task> Tasks { get; set; }

        public override string ToString()
        {
            return string.Format("Category[CategoryId:{0}, Name:{1}, ProjectId:{2}", CategoryId, Name, ProjectId);
        }
    }
}
