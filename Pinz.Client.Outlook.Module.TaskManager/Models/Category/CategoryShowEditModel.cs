using Com.Pinz.Client.Outlook.Module.TaskManager.Events;
using Com.Pinz.Client.Outlook.Service;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Models
{
    public class CategoryShowEditModel : BindableBase
    {
        public OutlookCategory Category { get; set; }

        private bool _isEditorEnabled = false;
        public bool IsEditorEnabled
        {
            get { return _isEditorEnabled; }
            set
            {
                SetProperty(ref this._isEditorEnabled, value);
            }
        }

        public DelegateCommand StartEditCategory { get; private set; }
        public DelegateCommand CancelEditCategory { get; private set; }
        public DelegateCommand UpdateCategory { get; private set; }

        private ICategoryService service;
        private IEventAggregator eventAggregator;
        private string originalCategoryName;

        [Inject]
        public CategoryShowEditModel(ICategoryService service, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.service = service;
            IsEditorEnabled = false;

            StartEditCategory = new DelegateCommand(OnStartEditCategory);
            CancelEditCategory = new DelegateCommand(OnCancelEditCategory);
            UpdateCategory = new DelegateCommand(OnUpdateCategory);

            OutlookCategoryEditStartedEvent categoryEditStartedEvent = eventAggregator.GetEvent<OutlookCategoryEditStartedEvent>();
            categoryEditStartedEvent.Subscribe(CategoryEditStartedEventHandler, ThreadOption.UIThread, false, c => c != Category);

            OutlookTaskEditStartedEvent taskEditStartedEvent = eventAggregator.GetEvent<OutlookTaskEditStartedEvent>();
            taskEditStartedEvent.Subscribe(CategoryEditStartedEventHandler);
        }

        private void CategoryEditStartedEventHandler(object category)
        {
            if (IsEditorEnabled)
                OnCancelEditCategory();
        }

        private void OnUpdateCategory()
        {
            service.Update(Category);
            IsEditorEnabled = false;
        }

        private void OnCancelEditCategory()
        {
            Category.Name = originalCategoryName;
            IsEditorEnabled = false;
        }

        private void OnStartEditCategory()
        {
            eventAggregator.GetEvent<OutlookCategoryEditStartedEvent>().Publish(Category);

            originalCategoryName = Category.Name;
            IsEditorEnabled = true;
        }
    }
}
