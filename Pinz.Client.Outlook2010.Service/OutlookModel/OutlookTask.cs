using Com.Pinz.Client.DomainModel.Model;

namespace Pinz.Client.Outlook2010.Service.OutlookModel
{
    public class OutlookTask : Task
    {
        public OutlookCategory Category { get; set; }
        public string EntryId { get; set; }
    }
}
