using Com.Pinz.Client.Commons.Prism;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.Outlook.Service.Model
{
    public class OutlookTask : BindableValidationBase
    {
        private string entryId;

        private string taskName;
        private string body;
        private bool complete;

        private string owner;

        private DateTime creationTime;
        private DateTime? dateCompleted;
        private DateTime? startDate;
        private DateTime? dueDate;

        private int actualWork;
        private TaskStatus status;
        private string priority;

        private OutlookCategory categories;
        private string companies;


        public string EntryId
        {
            get { return entryId; }
            set { SetProperty(ref this.entryId, value); }
        }

        [Required]
        public string TaskName
        {
            get { return taskName; }
            set { SetProperty(ref this.taskName, value); }
        }

        public string Body
        {
            get { return body; }
            set { SetProperty(ref this.body, value); }
        }

        public bool IsComplete
        {
            get { return complete; }
            set { SetProperty(ref this.complete, value); }
        }

        public string Owner
        {
            get { return owner; }
            set { SetProperty(ref this.owner, value); }
        }

        [Required]
        public DateTime CreationTime
        {
            get { return creationTime; }
            set { SetProperty(ref this.creationTime, value); }
        }

        public DateTime? DateCompleted
        {
            get { return dateCompleted; }
            set { SetProperty(ref this.dateCompleted, value); }
        }

        public DateTime? StartDate
        {
            get { return startDate; }
            set { SetProperty(ref this.startDate, value); }
        }

        public DateTime? DueDate
        {
            get { return dueDate; }
            set { SetProperty(ref this.dueDate, value); }
        }

        public int ActualWork
        {
            get { return actualWork; }
            set { SetProperty(ref this.actualWork, value); }
        }

        public TaskStatus Status
        {
            get { return status; }
            set { SetProperty(ref this.status, value); }
        }

        public string Priority
        {
            get { return priority; }
            set { SetProperty(ref this.priority, value); }
        }

        public OutlookCategory Category
        {
            get { return categories; }
            set { SetProperty(ref this.categories, value); }
        }

        public string Companies
        {
            get { return companies; }
            set { SetProperty(ref this.companies, value); }
        }

    }
}
