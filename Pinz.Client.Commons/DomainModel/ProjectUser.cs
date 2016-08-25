using Com.Pinz.DomainModel;

namespace Com.Pinz.Client.DomainModel
{
    public class ProjectUser : User, IProjectUser
    {
        private bool _IProjectAdmin;
        public bool IsProjectAdmin
        {
            get { return _IProjectAdmin; }
            set { SetProperty(ref this._IProjectAdmin, value); }
        }
    }
}
