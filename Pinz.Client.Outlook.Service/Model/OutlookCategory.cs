using Com.Pinz.Client.Commons.Prism;
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.Outlook.Service.Model
{
    public class OutlookCategory : BindableValidationBase
    {
        private string _name;

        [Required]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }
    }
}
