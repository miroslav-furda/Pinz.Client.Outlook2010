using Com.Pinz.Client.Outlook.Service;
using Com.Pinz.Client.Outlook.Service.Model;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        public DelegateCommand CreateTask { get; private set; }

        private ObservableCollection<OutlookTask> _tasks;
        public ObservableCollection<OutlookTask> Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                SetProperty(ref this._tasks, value);
            }
        }


        private OutlookCategory _category;
        public OutlookCategory Category
        {
            get
            {
                return _category;
            }
            set
            {
                SetProperty(ref this._category, value);
                if (value != null)
                    Tasks = service.ReadByCategory(Category);
                else
                    Tasks = null;
            }
        }

        private ITaskService service;

        [Inject]
        public TaskListModel(ITaskService service)
        {
            this.service = service;
            CreateTask = new DelegateCommand(OnCreateTask);
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            OutlookTask sourceItem = dropInfo.Data as OutlookTask;
            OutlookTask targetItem = dropInfo.TargetItem as OutlookTask;


            if (sourceItem != null && ((targetItem != null && sourceItem.Category != targetItem.Category) || targetItem == null))
            {
                System.Diagnostics.Debug.WriteLine("DragOver called with source:{0} and target:{1}", sourceItem, targetItem);
                //dropInfo.DestinationText = sourceItem.TaskName;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DragOver None");
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            OutlookTask sourceItem = dropInfo.Data as OutlookTask;
            OutlookTask targetItem = dropInfo.TargetItem as OutlookTask;

            service.MoveToCategory(sourceItem, Category);
        }

        private void OnCreateTask()
        {
            service.CreateNewTask(this.Category);
        }
    }
}
