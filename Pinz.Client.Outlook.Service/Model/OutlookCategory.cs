using Prism.Mvvm;

namespace Com.Pinz.Client.Outlook.Service.Model
{
    public class OutlookCategory : BindableBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }
    }
}
