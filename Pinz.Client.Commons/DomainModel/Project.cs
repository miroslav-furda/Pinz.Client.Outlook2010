﻿using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.DomainModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.DomainModel

{
    public class Project : BindableValidationBase, IProject
    {
        public Guid ProjectId { get; set; }

        private string _name;
        [Required]//(ErrorMessageResourceName = "Project_Name_Required", ErrorMessageResourceType = typeof(Resources))]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref this._description, value); }
        }

        private Guid _companyId;
        [Required]//(ErrorMessageResourceName = "Project_Company_Required", ErrorMessageResourceType = typeof(Resources))]
        public Guid CompanyId
        {
            get { return _companyId; }
            set { SetProperty(ref this._companyId, value); }
        }

        private ObservableCollection<User> _projectUsers;
        public ObservableCollection<User> ProjectUsers
        {
            get { return _projectUsers; }
            set { SetProperty(ref _projectUsers, value); }
        }

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        public Project()
        {
            ProjectUsers = new ObservableCollection<User>();
        }

        public override string ToString()
        {
            return string.Format("Project[ProjectId:{0}, Name:{1}, Description:{2}, CompanyId:{3}", ProjectId, Name, Description, CompanyId);
        }

    }
}
