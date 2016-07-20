using AutoMapper;
using Com.Pinz.Client.Outlook.Module.TaskManager.Events;
using Com.Pinz.Client.Outlook.Module.TaskManager.Properties;
using Com.Pinz.Client.Outlook.Service;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Models
{
    public class TaskEditModel : BindableBase
    {
        private OutlookTask _task;
        public OutlookTask Task
        {
            get
            {
                return _task;
            }
            set
            {
                SetProperty(ref this._task, value);
            }
        }

        private bool _editMode;
        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                SetProperty(ref this._editMode, value);
            }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public InteractionRequest<IConfirmation> DeleteConfirmation { get; private set; }

        private IEventAggregator eventAggregator;
        private ITaskService service;
        private OutlookTask originalTask;
        private IMapper mapper;

        [Inject]
        public TaskEditModel(ITaskService service, IEventAggregator eventAggregator,[Named("OutlookClientMapper")] IMapper mapper)
        {
            this.service = service;
            this.eventAggregator = eventAggregator;
            this.mapper = mapper;
            this.EditMode = false;
            this.originalTask = null;

            OutlookTaskEditStartedEvent taskEditStartEvent = eventAggregator.GetEvent<OutlookTaskEditStartedEvent>();
            taskEditStartEvent.Subscribe(StartEdit, ThreadOption.UIThread, false, t => t == Task);
            taskEditStartEvent.Subscribe(OnCancelExecute, ThreadOption.UIThread, false, t => t != Task);
            OutlookCategoryEditStartedEvent categoryEditEvent = eventAggregator.GetEvent<OutlookCategoryEditStartedEvent>();
            categoryEditEvent.Subscribe(OnCancelExecute);

            OkCommand = new DelegateCommand(OnOkExecute);
            CancelCommand = new DelegateCommand(OnCancelExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            this.DeleteConfirmation = new InteractionRequest<IConfirmation>();
        }

        private void OnDeleteExecute()
        {
            this.DeleteConfirmation.Raise(new Confirmation
            {
                Title = Resources.DeleteConfirmation_Title,
                Content = Resources.DeleteConfirmation_Content
            }, (dialog) =>
            {
                if (dialog.Confirmed)
                {
                    EditMode = false;
                    eventAggregator.GetEvent<OutlookTaskEditFinishedEvent>().Publish(Task);
                    service.Delete(this.Task);
                }
            });
        }

        private void OnCancelExecute(object obj)
        {
            if (EditMode)
                OnCancelExecute();
        }

        private void OnCancelExecute()
        {
            mapper.Map(originalTask, Task);
            EditMode = false;
            eventAggregator.GetEvent<OutlookTaskEditFinishedEvent>().Publish(Task);
        }

        private void OnOkExecute()
        {
            service.Update(this.Task);
            EditMode = false;
            eventAggregator.GetEvent<OutlookTaskEditFinishedEvent>().Publish(Task);
        }

        private void StartEdit(OutlookTask obj)
        {
            originalTask = mapper.Map<OutlookTask>(Task);
            EditMode = true;
        }
    }
}
