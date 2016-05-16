using Com.Pinz.Client.Outlook.Module.TaskManager.Events;
using Com.Pinz.Client.Outlook.Service;
using Com.Pinz.Client.Outlook.Service.Model;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Models
{
    public class TaskShowEditModel : BindableBase
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
                value.PropertyChanged += Task_PropertyChanged;
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


        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand<bool?> CompleteCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }

        private ITaskService service;
        private IEventAggregator eventAggregator;


        [Inject]
        public TaskShowEditModel(ITaskService service, IEventAggregator eventAggregator)
        {
            this.service = service;
            this.eventAggregator = eventAggregator;
            EditMode = false;

            this.StartCommand = new DelegateCommand(OnStart, CanStart);
            this.CompleteCommand = new DelegateCommand<bool?>(this.OnComplete);
            this.EditCommand = new DelegateCommand(OnEdit, CanEdit);

            OutlookTaskEditFinishedEvent taskEditFinishedEvent = eventAggregator.GetEvent<OutlookTaskEditFinishedEvent>();
            taskEditFinishedEvent.Subscribe(StopEdit, ThreadOption.UIThread, false, t => t == Task);

        }

        private bool CanEdit()
        {
            return !EditMode;
        }

        private void StopEdit(OutlookTask obj)
        {
            EditMode = false;
            EditCommand.RaiseCanExecuteChanged();
        }

        private void OnEdit()
        {
            EditMode = true;
            eventAggregator.GetEvent<OutlookTaskEditStartedEvent>().Publish(Task);
            EditCommand.RaiseCanExecuteChanged();
        }

        private void OnComplete(bool? selected)
        {
            if (selected == true)
            {
                service.CompleteTask(Task);
            }
            else if (selected == false)
            {
                service.StartTask(Task);
            }
            CompleteCommand.RaiseCanExecuteChanged();
            StartCommand.RaiseCanExecuteChanged();
        }

        private void OnStart()
        {
            service.StartTask(Task);
            StartCommand.RaiseCanExecuteChanged();
        }

        private bool CanStart()
        {
            return TaskStatus.TaskNotStarted.Equals(this.Task.Status);
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ("Status".Equals(e.PropertyName))
                StartCommand.RaiseCanExecuteChanged();
        }
    }
}
