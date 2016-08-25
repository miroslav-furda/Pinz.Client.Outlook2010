using Com.Pinz.Client.DomainModel;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Model
{
    public class ApplicationGlobalModel : BindableBase
    {
        public User CurrentUser { get; set; }

        private bool _isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get
            {
                return _isUserLoggedIn;
            }
            set
            {
                SetProperty(ref this._isUserLoggedIn, value);
            }
        }

        private Dictionary<Project, ObservableCollection<User>> _projectUsers { get; set; }

        public ApplicationGlobalModel()
        {
            IsUserLoggedIn = false;
            _projectUsers = new Dictionary<Project, ObservableCollection<User>>();
        }

        public ObservableCollection<User> GetUsersForProject(Project project)
        {
            if(!_projectUsers.ContainsKey(project))
            {
                _projectUsers.Add(project, new ObservableCollection<User>());
            }
            return _projectUsers[project];
        }

    }
}
