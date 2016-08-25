using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.DomainModel;
using Com.Pinz.Client.Model;
using Prism.Events;
using Com.Pinz.Client.Module.TaskManager.Events;
using System.Linq;
using Com.Pinz.Client.DomainModel;
using System;
using Com.Pinz.Client.Commons.Event;
using AutoMapper;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        public AwaitableDelegateCommand CreateTask { get; private set; }

        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }
        

        private Category _category;
        public Category Category
        {
            get { return _category; }
            set
            {
                if(SetProperty(ref _category, value))
                {
                    Tasks.Clear();
                    Tasks.AddRange(Category.Tasks);
                }
                //LoadTasks();
            }
        }

        private readonly ITaskRemoteService _service;
        private readonly TaskFilter _taskFilter;
        //private List<DomainModel.Task> _allTasksFromServer;
        private User _currentUser;
        private IEventAggregator _eventAggregator;
        private readonly IMapper _mapper;

        [Inject]
        public TaskListModel(ITaskRemoteService service, TaskFilter filter, ApplicationGlobalModel applicationGlobalModel, 
            IEventAggregator eventAggregator, [Named("WpfClientMapper")] IMapper mapper)
        {
            this._service = service;
            this._taskFilter = filter;
            this._eventAggregator = eventAggregator;
            this.Tasks = new ObservableCollection<Task>();
            CreateTask = new AwaitableDelegateCommand(OnCreateTask);
            this._taskFilter.PropertyChanged += Filter_PropertyChanged;
            _currentUser = applicationGlobalModel.CurrentUser;
            _mapper = mapper;

            TaskDeletedEvent taskDeletedEvent = eventAggregator.GetEvent<TaskDeletedEvent>();
            taskDeletedEvent.Subscribe(OnDeleteTask, ThreadOption.UIThread, false, t => Category != null && t.CategoryId == Category.CategoryId);
        }

        private void OnDeleteTask(Task taskToDelete)
        {
            var toDelete = Category.Tasks.Where(t => t.TaskId == taskToDelete.TaskId).First();
            Category.Tasks.Remove(taskToDelete);
            Tasks.Remove(taskToDelete);
            //var allTaskFromServerToDelete = _allTasksFromServer.Where(t => t.TaskId == taskToDelete.TaskId).First();
            //_allTasksFromServer.Remove(allTaskFromServerToDelete);
        }


        #region private Filter

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Category.Tasks != null)
                updateTaskObservableCollection();
        }

        private bool filterTasks(DomainModel.Task taskitem)
        {
            bool retval = true;

            if (!_taskFilter.Complete)
            {
                retval = taskitem.Status != TaskStatus.TaskComplete;
            }

            if (retval && _taskFilter.DueToday)
            {
                System.DateTime today = System.DateTime.Today;
                retval = taskitem.DueDate.Equals(today);
            }

            if (retval && _taskFilter.InProgress && _taskFilter.NotStarted)
            {
                retval = taskitem.Status == TaskStatus.TaskInProgress ||
                   taskitem.Status == TaskStatus.TaskNotStarted;
            }
            else if (retval && _taskFilter.InProgress)
            {
                retval = taskitem.Status == TaskStatus.TaskInProgress;
            }
            else if (retval && _taskFilter.NotStarted)
            {
                retval = taskitem.Status == TaskStatus.TaskNotStarted;
            }

            if (retval && _taskFilter.MyTasks)
            {
                retval = taskitem.UserId == _currentUser.UserId;

            }
            return retval;
        }
        #endregion


        #region Drag and Drop

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as Task;
            var targetItem = dropInfo.TargetItem as Task;


            if (sourceItem != null && ((targetItem != null && sourceItem.CategoryId != targetItem.CategoryId) || targetItem == null))
            {
                Debug.WriteLine("DragOver called with source:{0} and target:{1}", sourceItem, targetItem);
                //dropInfo.DestinationText = sourceItem.TaskName;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                Debug.WriteLine("DragOver None");
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        async void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as Task;
            var targetItem = dropInfo.TargetItem as Task;

            try
            {
                Task originalTask = _mapper.Map<Task>(sourceItem);

                await _service.MoveTaskToCategoryAsync(sourceItem, Category);

                _eventAggregator.GetEvent<TaskDeletedEvent>().Publish(originalTask);
                Category.Tasks.Add(sourceItem);
                //_allTasksFromServer.Add(sourceItem);
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }

        }

        #endregion

        private async System.Threading.Tasks.Task OnCreateTask()
        {
            try
            {
                var newTask = await _service.CreateTaskInCategoryAsync(Category);
                Tasks.Add(newTask);
                Category.Tasks.Add(newTask);
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
        }

        /*
        private async System.Threading.Tasks.Task LoadTasks()
        {
            if (Category != null)
            {
                try
                {
                    _allTasksFromServer = await _service.ReadAllTasksByCategoryAsync(Category);
                    updateTaskObservableCollection();
                    Category.Tasks = Tasks;
                }
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
            else
            {
                Tasks.Clear();
            }
        }
        */

        private void updateTaskObservableCollection()
        {
            Tasks.Clear();
            foreach (var task in Category.Tasks)
            {
                if (filterTasks(task))
                    Tasks.Add(task);
            }
        }
    }
}